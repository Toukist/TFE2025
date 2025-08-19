using Dior.Library.DTO;
using Dior.Library.Service.DAO;
using Dior.Service.Services;
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

        // =====================================================================================
        // ADO.NET (SP)
        // =====================================================================================

        public int Add(User user, string editBy)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_AddUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                // TODO: Ajoute tes paramètres ici (Username, FirstName, LastName, Email, Phone, TeamId, IsActive, editBy,...)
                // cmd.Parameters.AddWithValue("@Username", user.Name); // legacy: Name = Username
                // ...
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
            catch
            {
                return -1;
            }
        }

        public void Set(User user, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_SetUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            // TODO: Ajoute tes paramètres ici
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<User> GetList(List<int> userIds) // garde la signature (int) pour compat, même si Id est bigint en DB
        {
            var users = new List<User>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetUsers", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                // TODO: Si sp_GetUsers accepte une liste d'IDs, ajoute le param ici.

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // FIX: Id en bigint -> GetInt64
                    var id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0L : reader.GetInt64(reader.GetOrdinal("Id"));

                    // legacy: le BO "User" expose souvent "Name" pour le login → y stocke "Username"
                    var username = reader["Username"]?.ToString() ?? string.Empty;

                    var user = new User
                    {
                        // Si ton BO est en long -> Id = id; sinon cas limite : (int)id
                        Id = (int)id, // ⚠️ garde int si ton BO est encore en int; sinon passe en long
                        Name = username, // FIX: Name = Username (legacy)
                        LastName = reader["LastName"]?.ToString() ?? "",
                        FirstName = reader["FirstName"]?.ToString() ?? "",
                        IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive")) && reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        LastEditBy = reader["LastEditBy"]?.ToString() ?? "",
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                        Email = reader["Email"]?.ToString() ?? "",
                        Phone = reader["Phone"]?.ToString() ?? "",
                        // FIX: TeamId nullable
                        TeamId = reader.IsDBNull(reader.GetOrdinal("TeamId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TeamId"))
                    };
                    users.Add(user);
                }
            }
            catch
            {
                // log si besoin
            }
            return users;
        }

        public List<User> GetList() => GetList(null);

        public User Get(int id)
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
                    var dbId = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0L : reader.GetInt64(reader.GetOrdinal("Id"));
                    var username = reader["Username"]?.ToString() ?? string.Empty;

                    return new User
                    {
                        Id = (int)dbId, // ⚠️ garde int si ton BO est encore en int
                        Name = username, // FIX: Name = Username (legacy)
                        LastName = reader["LastName"]?.ToString() ?? "",
                        FirstName = reader["FirstName"]?.ToString() ?? "",
                        IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive")) && reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        LastEditBy = reader["LastEditBy"]?.ToString() ?? "",
                        LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                        Email = reader["Email"]?.ToString() ?? "",
                        Phone = reader["Phone"]?.ToString() ?? "",
                        TeamId = reader.IsDBNull(reader.GetOrdinal("TeamId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TeamId"))
                    };
                }
            }
            catch
            {
                // log si besoin
            }
            return null;
        }

        public void Del(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DelUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Remplacement de "_context.Accesses" par "_context.Access" dans la requête LINQ de GetAllWithTeam
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
                join a in _context.Access on ua.AccessId equals a.Id into aJoin // <-- FIX ici
                from a in aJoin.DefaultIfEmpty()
                select new
                {
                    u.Id,
                    UserName = u.Name, // legacy propriété EF, correspond à Username
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
                        UserName = any.UserName,
                        LastName = any.LastName,
                        FirstName = any.FirstName,
                        IsActive = any.IsActive,
                        Email = any.Email,
                        Phone = any.Phone,
                        TeamId = any.TeamId,
                        TeamName = any.TeamName ?? string.Empty,
                        // FIX: string? côté DTO
                        BadgePhysicalNumber = ParseBadgeNumber(g.Select(x => x.Badge).FirstOrDefault())
                    };
                })
                .ToList();

            // FIX: ajouter les rôles (liste de RoleDefinitionDto) si nécessaire
            var roleMap =
                (from ur in _context.UserRoles
                 join rd in _context.RoleDefinitions on ur.RoleDefinitionId equals rd.Id
                 select new { ur.UserId, Role = rd })
                .AsEnumerable()
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new RoleDefinitionDto
                    {
                        Id = x.Role.Id,
                        Name = x.Role.Name,
                        Description = x.Role.Description,
                        ParentRoleId = x.Role.ParentRoleId,
                        IsActive = x.Role.IsActive,
                        CreatedBy = x.Role.CreatedBy,
                        LastEditBy = x.Role.LastEditBy
                    }).ToList()
                );

            foreach (var u in list)
            {
                if (roleMap.TryGetValue((int)u.Id, out var roleDtos))
                {
                    // Convertir RoleDefinitionDto en List<string> pour compatibilité
                    u.Roles = roleDtos.Select(r => r.Name).ToList();
                }
            }

            return list;
        }

        public List<string> GetUserRoles(long userId)
        {
            return (from ur in _context.UserRoles
                    join rd in _context.RoleDefinitions on ur.RoleDefinitionId equals rd.Id
                    where ur.UserId == userId
                    select rd.Name).ToList();
        }

        // Front Angular : users + roles (noms), badge, team
        public List<UserFullDto> GetUsersWithRoles()
        {
            var users = new Dictionary<long, UserFullDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetUsersWithRoles", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // FIX: Id bigint
                var userId = reader.GetInt64(reader.GetOrdinal("UserId"));

                if (!users.TryGetValue(userId, out var user))
                {
                    user = new UserFullDto
                    {
                        Id = userId,
                        FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                        LastName = reader["LastName"]?.ToString() ?? string.Empty,
                        Email = reader["Email"]?.ToString() ?? string.Empty,
                        Phone = reader["Phone"]?.ToString(),
                        // FIX: badge int? (cohérent avec le DTO)
                        BadgePhysicalNumber = reader.IsDBNull(reader.GetOrdinal("BadgePhysicalNumber"))
                            ? null
                            : Convert.ToInt32(reader["BadgePhysicalNumber"]),
                        TeamName = reader["TeamName"]?.ToString(),
                        TeamId = reader["TeamId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TeamId"]),
                        Roles = new List<string>() // Corrigé: List<string>
                    };
                    users.Add(userId, user);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("Roles")))
                {
                    var rolesCsv = reader.GetString(reader.GetOrdinal("Roles"));
                    var roles = rolesCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var role in roles)
                    {
                        if (!string.IsNullOrWhiteSpace(role) && user.Roles != null && !user.Roles.Contains(role))
                            user.Roles.Add(role); // Corrigé: Add string directement
                    }
                }
            }

            return users.Values.ToList();
        }

        // Méthode helper pour convertir le badge string en int?
        private static int? ParseBadgeNumber(string? badge)
        {
            if (string.IsNullOrWhiteSpace(badge)) return null;
            return int.TryParse(badge, out int result) ? result : null;
        }
    }
}