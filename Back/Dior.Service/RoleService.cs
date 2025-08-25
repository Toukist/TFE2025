using Dior.Data.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service
{
    public interface IRoleService
    {
        List<RoleDefinitionDto> GetRolesByUserId(long userId);
    }

    public class RoleService : IRoleService
    {
        private readonly string _connectionString;
        public RoleService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DIOR_DB")
                ?? throw new InvalidOperationException("ConnectionStrings:DIOR_DB manquante");
        }

        public List<RoleDefinitionDto> GetRolesByUserId(long userId)
        {
            var roles = new List<RoleDefinitionDto>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand(@"SELECT rd.Id, rd.Name, rd.Description, rd.ParentRoleId, rd.IsActive, rd.CreatedAt, rd.CreatedBy, rd.LastEditAt, rd.LastEditBy FROM UserRole ur INNER JOIN RoleDefinition rd ON ur.RoleDefinitionId = rd.Id WHERE ur.UserId = @userId", conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new RoleDefinitionDto
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    ParentRoleId = reader.IsDBNull("ParentRoleId") ? null : reader.GetInt32("ParentRoleId"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetString("CreatedBy"),
                    LastEditAt = reader.IsDBNull("LastEditAt") ? null : reader.GetDateTime("LastEditAt"),
                    LastEditBy = reader.IsDBNull("LastEditBy") ? null : reader.GetString("LastEditBy")
                });
            }
            return roles;
        }
    }
}
