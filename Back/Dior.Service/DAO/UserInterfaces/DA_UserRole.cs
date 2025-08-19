using Dior.Library.Service.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_UserRole : IDA_UserRole
    {
        private readonly string _connectionString;

        public DA_UserRole(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public long Add(UserRole userRole, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_User_Role_Add", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_RoleDefinitionID", userRole.RoleDefinitionId);
            cmd.Parameters.AddWithValue("@PR_UserID", userRole.UserId);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy ?? (object)DBNull.Value);

            var outParam = new SqlParameter("@PR_ID", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            return outParam.Value != DBNull.Value ? (long)outParam.Value : -1;
        }

        public void Set(UserRole userRole, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_User_Role_Set", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_RoleDefinitionID", userRole.RoleDefinitionId);
            cmd.Parameters.AddWithValue("@PR_UserID", userRole.UserId);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<UserRole> GetList()
        {
            var list = new List<UserRole>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_User_Role_GetList", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var userRole = new UserRole
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    RoleDefinitionId = reader.GetInt32(reader.GetOrdinal("RoleDefinitionID")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserID")),
                    ExpiresAt = reader.IsDBNull(reader.GetOrdinal("ExpiresAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ExpiresAt")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                    LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                    LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
                };
                list.Add(userRole);
            }

            return list;
        }

        public void Del(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_User_Role_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public UserRole Get(long id)
        {
            throw new NotImplementedException();
        }
    }
}
