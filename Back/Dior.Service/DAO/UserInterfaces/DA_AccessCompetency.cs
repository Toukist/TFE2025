using Dior.Library.BO.UserInterface;
using Dior.Library.Service.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces

{
    public class DA_AccessCompetency : IDA_AccessCompetency
    {
        private readonly string _connectionString;

        public DA_AccessCompetency(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public long Add(AccessCompetency accessCompetency, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.SP_AccessCompetency_Add", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_Name", accessCompetency.Name);
            // cmd.Parameters.AddWithValue("@PR_Description", accessCompetency.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_IsActive", accessCompetency.IsActive);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy);

            var idParam = new SqlParameter("@PR_ID", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(idParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            return idParam.Value != DBNull.Value ? (long)idParam.Value : -1;
        }

        public void Set(AccessCompetency accessCompetency, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.SP_AccessCompetency_Set", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", accessCompetency.Id);
            cmd.Parameters.AddWithValue("@PR_Name", accessCompetency.Name);
            //cmd.Parameters.AddWithValue("@PR_Description", accessCompetency.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PR_IsActive", accessCompetency.IsActive);
            cmd.Parameters.AddWithValue("@PR_EditBy", editBy);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public AccessCompetency Get(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.SP_AccessCompetency_Get", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            conn.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new AccessCompetency
                {
                    Id = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("Id"))),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    //Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    //      ? null
                    //      : reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };
            }

            return null;
        }

        public List<AccessCompetency> GetList()
        {
            var list = new List<AccessCompetency>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.SP_AccessCompetency_GetList", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var accessCompetency = new AccessCompetency
                {
                    Id = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("Id"))),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    //   Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    //      ? null
                    //      : reader.GetString(reader.GetOrdinal("Description")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };

                list.Add(accessCompetency);
            }

            return list;
        }

        public void Del(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.SP_AccessCompetency_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@PR_ID", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

