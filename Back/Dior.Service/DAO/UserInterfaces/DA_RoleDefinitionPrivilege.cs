using Dior.Library.Service.DAO;
using Dior.Library.BO.UserInterface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_RoleDefinitionPrivilege : IDA_RoleDefinitionPrivilege
    {
        private readonly string _connectionString;

        public DA_RoleDefinitionPrivilege(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public long Add(RoleDefinitionPrivilege link, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Privilege_Add", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_RoleDefinitionID", link.RoleDefinitionId);
            cmd.Parameters.AddWithValue("@PR_PrivilegeID", link.PrivilegeId);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy ?? (object)DBNull.Value);

            var outParam = new SqlParameter("@PR_ID", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            return outParam.Value != DBNull.Value ? Convert.ToInt64(outParam.Value) : -1;
        }

        public void Set(RoleDefinitionPrivilege link, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Privilege_Set", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", link.Id);
            cmd.Parameters.AddWithValue("@PR_RoleDefinitionID", link.RoleDefinitionId);
            cmd.Parameters.AddWithValue("@PR_PrivilegeID", link.PrivilegeId);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Del(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Privilege_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public RoleDefinitionPrivilege Get(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Privilege_Get", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public List<RoleDefinitionPrivilege> GetList()
        {
            var list = new List<RoleDefinitionPrivilege>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_RoleDefinition_Privilege_GetList", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapFromReader(reader));
            }

            return list;
        }

        private RoleDefinitionPrivilege MapFromReader(SqlDataReader reader)
        {
            return new RoleDefinitionPrivilege
            {
                Id = Convert.ToInt32(reader["Id"]),
                RoleDefinitionId = Convert.ToInt32(reader["RoleDefinitionId"]),
                PrivilegeId = Convert.ToInt32(reader["PrivilegeId"]),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
            };
        }
    }
}