using Dior.Library.DTO;
using Dior.Library.Service.DAO;
using Dior.Service.Services;
using Dior.Library.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_User : IDA_User
    {
        private readonly string _connectionString;
        private readonly DiorDbContext _context;

        public DA_User(IConfiguration configuration, DiorDbContext context)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
            _context = context;
        }

        // Implémentation de la nouvelle interface IDA_User
        public User GetUserByUsername(string username)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetUserByUsername", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
            }
            catch
            {
                // log si besoin
            }
            return null;
        }

        public User GetUserById(long id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
            }
            catch
            {
                // log si besoin
            }
            return null;
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = new List<User>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetAllUsers", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(MapUser(reader));
                }
            }
            catch
            {
                // log si besoin
            }
            return users;
        }

        public void CreateUser(User user)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_AddUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TeamId", user.TeamId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", user.CreatedBy);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // log si besoin
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_UpdateUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", user.Id);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TeamId", user.TeamId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@LastEditBy", user.LastEditBy);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // log si besoin
            }
        }

        public void DeleteUser(long id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_DeleteUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // log si besoin
            }
        }

        // Méthode helper pour mapper un SqlDataReader vers User
        private User MapUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                Username = reader["Username"]?.ToString() ?? string.Empty,
                PasswordHash = reader["PasswordHash"]?.ToString() ?? string.Empty,
                FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                LastName = reader["LastName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                TeamId = reader.IsDBNull(reader.GetOrdinal("TeamId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TeamId")),
                IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive")) && reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader["CreatedBy"]?.ToString() ?? string.Empty,
                LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                LastEditBy = reader["LastEditBy"]?.ToString()
            };
        }

        // Méthodes legacy pour compatibilité - peuvent être supprimées plus tard
        public List<UserDto> GetAllWithTeam()
        {
            // FIX: ajout badge + roles (si dispo dans le modèle EF)
            var query =
                from u in _context.User
                join t in _context.Teams on u.TeamId equals t.Id into teamJoin
                from t in teamJoin.DefaultIfEmpty()
                    // Badge via UserAccess/Access si disponible
                join ua in _context.UserAccesses on u.Id equals ua.UserId into uaJoin
                from ua in uaJoin.DefaultIfEmpty()
                join a in _context.Access on ua.AccessId equals a.Id into aJoin
                from a in aJoin.DefaultIfEmpty()
                select new
                {
                    u.Id,
                    UserName = u.Username,
                    u.LastName,
                    u.FirstName,
                    u.IsActive,
                    u.Email,
                    u.Phone,
                    u.TeamId,
                    TeamName = t != null ? t.Name : null,
                    Badge = a != null ? a.BadgePhysicalNumber : null
                };

            // GroupBy Id pour prendre le 1er badge si plusieurs
            var list = query
                .AsEnumerable()
                .GroupBy(x => x.Id)
                .Select(g =>
                {
                    var any = g.First();
                    return new UserDto
                    {
                        Id = any.Id,
                        Username = any.UserName,
                        LastName = any.LastName,
                        FirstName = any.FirstName,
                        IsActive = any.IsActive,
                        Email = any.Email,
                        Phone = any.Phone,
                        TeamId = any.TeamId,
                        TeamName = any.TeamName ?? string.Empty,
                        BadgePhysicalNumber = ParseBadgeNumber(g.Select(x => x.Badge).FirstOrDefault())
                    };
                })
                .ToList();

            return list;
        }

        public List<string> GetUserRoles(long userId)
        {
            return (from ur in _context.UserRoles
                    join rd in _context.RoleDefinitions on ur.RoleDefinitionId equals rd.Id
                    where ur.UserId == userId
                    select rd.Name).ToList();
        }

        // Méthode helper pour convertir le badge string en int?
        private static int? ParseBadgeNumber(string? badge)
        {
            if (string.IsNullOrWhiteSpace(badge)) return null;
            return int.TryParse(badge, out int result) ? result : null;
        }
    }
}