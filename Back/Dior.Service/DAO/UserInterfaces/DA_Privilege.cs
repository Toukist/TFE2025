using Dior.Library.BO.UserInterface;
using Dior.Library.Service.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_Privilege : IDA_Privilege
    {
        private readonly string _connectionString;

        public DA_Privilege(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public long Add(Privilege privilege, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Privilege_Add", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_Name", privilege.Name);
            cmd.Parameters.AddWithValue("@PR_Description", privilege.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_IsActive", privilege.IsActive);
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

        public void Set(Privilege privilege, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Privilege_Set", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", privilege.Id);
            cmd.Parameters.AddWithValue("@PR_Name", privilege.Name);
            cmd.Parameters.AddWithValue("@PR_Description", privilege.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_IsActive", privilege.IsActive);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public Privilege Get(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Privilege_Get", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Privilege
                {
                    Id = (int)reader.GetInt64(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };
            }

            return null;
        }

        public List<Privilege> GetList()
        {
            var list = new List<Privilege>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Privilege_GetList", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var privilege = new Privilege
                {
                    Id = (int)reader.GetInt64(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };

                list.Add(privilege);
            }

            return list;
        }

        public void Del(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_Privilege_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Entities overloads: map to BO then reuse existing logic
        public long Add(Dior.Library.Entities.Privilege privilege, string editBy)
        {
            var bo = new Privilege
            {
                Id = (int)privilege.Id,
                Name = privilege.Name,
                Description = privilege.Description,
                IsActive = privilege.IsActive
            };
            return Add(bo, editBy);
        }

        public void Set(Dior.Library.Entities.Privilege privilege, string editBy)
        {
            var bo = new Privilege
            {
                Id = (int)privilege.Id,
                Name = privilege.Name,
                Description = privilege.Description,
                IsActive = privilege.IsActive
            };
            Set(bo, editBy);
        }
    }
}
