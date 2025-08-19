using Dior.Library.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
            using var cmd = new SqlCommand(@"SELECT rd.Id, rd.Name FROM UserRole ur INNER JOIN RoleDefinition rd ON ur.RoleDefinitionId = rd.Id WHERE ur.UserId = @userId", conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new RoleDefinitionDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return roles;
        }
    }
}
