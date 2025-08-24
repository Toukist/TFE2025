using Microsoft.Data.SqlClient;
using Dapper;
using Dior.Library.DTO.User;
using Dior.Library.DTO.Role;
using Dior.Library.DTO.Auth;

namespace Dior.Service.Host.Services
{
    /// <summary>
    /// Service d'authentification pour le syst�me Dior - Acc�s direct DB
    /// </summary>
    public class AuthenticationService
    {
        private readonly string _connectionString;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IConfiguration config, ILogger<AuthenticationService> logger)
        {
            _connectionString = config.GetConnectionString("DIOR_DB") ?? 
                throw new ArgumentException("Connection string DIOR_DB is required");
            _logger = logger;
        }

        /// <summary>
        /// Authentifie un utilisateur par badge physique
        /// </summary>
        public async Task<UserDto?> AuthenticateByBadgeAsync(string badgePhysicalNumber)
        {
            if (string.IsNullOrEmpty(badgePhysicalNumber))
                return null;

            try
            {
                using var conn = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT DISTINCT u.ID, u.Username, u.FirstName, u.LastName, 
                           u.Email, u.Phone, u.IsActive, u.TeamId
                    FROM [USER] u
                    INNER JOIN [USER_ACCESS] ua ON u.ID = ua.UserId
                    INNER JOIN [ACCESS] a ON ua.AccessId = a.ID
                    WHERE a.BadgePhysicalNumber = @Badge AND u.IsActive = 1";
                
                var user = await conn.QueryFirstOrDefaultAsync<UserDto>(sql, new { Badge = badgePhysicalNumber });
                
                if (user != null)
                {
                    user.BadgePhysicalNumber = badgePhysicalNumber;
                    _logger.LogInformation("Utilisateur authentifi� par badge: {Username}", user.Username);
                }
                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'authentification par badge: {Badge}", badgePhysicalNumber);
                return null;
            }
        }

        /// <summary>
        /// Authentifie un utilisateur par username/password
        /// </summary>
        public async Task<UserDto?> AuthenticateByCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            try
            {
                using var conn = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT ID, Username, FirstName, LastName, Email, Phone, 
                           IsActive, TeamId, passwordHash
                    FROM [USER] 
                    WHERE Username = @Username AND IsActive = 1";
                
                var userWithHash = await conn.QueryFirstOrDefaultAsync<dynamic>(sql, new { Username = username });
                
                if (userWithHash == null)
                {
                    _logger.LogWarning("Tentative de connexion avec un utilisateur inexistant: {Username}", username);
                    return null;
                }
                
                // V�rifier le mot de passe avec BCrypt
                if (!BCrypt.Net.BCrypt.Verify(password, userWithHash.passwordHash))
                {
                    _logger.LogWarning("Mot de passe incorrect pour l'utilisateur: {Username}", username);
                    return null;
                }

                var user = new UserDto
                {
                    Id = userWithHash.ID,
                    Username = userWithHash.Username,
                    FirstName = userWithHash.FirstName,
                    LastName = userWithHash.LastName,
                    Email = userWithHash.Email,
                    Phone = userWithHash.Phone,
                    IsActive = userWithHash.IsActive,
                    TeamId = userWithHash.TeamId
                };

                _logger.LogInformation("Utilisateur authentifi� par credentials: {Username}", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'authentification par credentials: {Username}", username);
                return null;
            }
        }

        /// <summary>
        /// Charge les r�les d'un utilisateur
        /// </summary>
        public async Task<List<RoleDefinitionDto>> GetUserRolesAsync(long userId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT rd.id as Id, rd.name as Name, rd.description as Description, rd.isActive as IsActive
                    FROM [ROLE_DEFINITION] rd
                    INNER JOIN [USER_ROLE] ur ON rd.id = ur.RoleDefinitionID
                    WHERE ur.UserID = @UserId AND rd.isActive = 1";
                
                var roles = await conn.QueryAsync<RoleDefinitionDto>(sql, new { UserId = userId });
                return roles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la r�cup�ration des r�les pour l'utilisateur: {UserId}", userId);
                return new List<RoleDefinitionDto>();
            }
        }

        /// <summary>
        /// Charge les comp�tences d'acc�s d'un utilisateur
        /// </summary>
        public async Task<List<string>> GetUserAccessCompetenciesAsync(long userId)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                const string sql = @"
                    SELECT DISTINCT a.AccessName
                    FROM [ACCESS] a
                    INNER JOIN [USER_ACCESS] ua ON a.ID = ua.AccessId
                    WHERE ua.UserId = @UserId";
                
                var competencies = await conn.QueryAsync<string>(sql, new { UserId = userId });
                return competencies.Where(c => !string.IsNullOrEmpty(c)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la r�cup�ration des comp�tences d'acc�s pour l'utilisateur: {UserId}", userId);
                return new List<string>();
            }
        }

        /// <summary>
        /// Charge le nom de l'�quipe d'un utilisateur
        /// </summary>
        public async Task<string?> GetUserTeamNameAsync(int? teamId)
        {
            if (!teamId.HasValue) return null;

            try
            {
                using var conn = new SqlConnection(_connectionString);
                const string sql = @"SELECT Name FROM [TEAM] WHERE Id = @TeamId";
                
                return await conn.QueryFirstOrDefaultAsync<string>(sql, new { TeamId = teamId.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la r�cup�ration du nom d'�quipe: {TeamId}", teamId);
                return null;
            }
        }

        /// <summary>
        /// Change le mot de passe d'un utilisateur
        /// </summary>
        public async Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                
                // V�rifier le mot de passe actuel
                const string getCurrentPasswordSql = @"
                    SELECT passwordHash FROM [USER] WHERE ID = @UserId AND IsActive = 1";
                
                var currentHashedPassword = await conn.QueryFirstOrDefaultAsync<string>(
                    getCurrentPasswordSql, new { UserId = userId });
                
                if (currentHashedPassword == null || !BCrypt.Net.BCrypt.Verify(currentPassword, currentHashedPassword))
                {
                    _logger.LogWarning("Tentative de changement de mot de passe avec un mot de passe actuel incorrect: {UserId}", userId);
                    return false;
                }

                // Mettre � jour avec le nouveau mot de passe
                var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                const string updatePasswordSql = @"
                    UPDATE [USER] 
                    SET passwordHash = @NewPassword, LastEditAt = GETUTCDATE(), LastEditBy = 'Self'
                    WHERE ID = @UserId";
                
                var rowsAffected = await conn.ExecuteAsync(updatePasswordSql, 
                    new { NewPassword = newHashedPassword, UserId = userId });

                _logger.LogInformation("Mot de passe chang� pour l'utilisateur: {UserId}", userId);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// V�rifie si un utilisateur existe par email
        /// </summary>
        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                const string sql = @"SELECT COUNT(1) FROM [USER] WHERE Email = @Email";
                
                var count = await conn.QueryFirstOrDefaultAsync<int>(sql, new { Email = email });
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la v�rification d'existence utilisateur: {Email}", email);
                return false;
            }
        }
    }
}