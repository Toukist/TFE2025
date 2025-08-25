using Dior.Library.Entities;
using Dior.Library.Service.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_UserAccessCompetency : IDA_UserAccessCompetency
    {
        private readonly string _connectionString;

        public DA_UserAccessCompetency(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public int Add(UserAccessCompetency userAccessCompetency, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_UserAccessCompetency_Add", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UAC_AccessCompetency", userAccessCompetency.AccessCompetencyId);
            cmd.Parameters.AddWithValue("@UAC_UserID", userAccessCompetency.UserId);
            cmd.Parameters.AddWithValue("@UAC_LastEditBy", editBy ?? (object)DBNull.Value);

            var outParam = new SqlParameter("@UAC_ID", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            return outParam.Value != DBNull.Value ? Convert.ToInt32(outParam.Value) : -1;
        }

        public void Set(UserAccessCompetency userAccessCompetency, string editBy)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_UserAccessCompetency_Set", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UAC_ID", userAccessCompetency.Id);
            cmd.Parameters.AddWithValue("@UAC_AccessCompetency", userAccessCompetency.AccessCompetencyId);
            cmd.Parameters.AddWithValue("@UAC_UserID", userAccessCompetency.UserId);
            cmd.Parameters.AddWithValue("@UAC_LastEditBy", editBy ?? (object)DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public UserAccessCompetency Get(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_UserAccessCompetency_Get", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UAC_ID", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new UserAccessCompetency
                {
<<<<<<< Updated upstream
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    AccessCompetencyId = reader.GetInt32(reader.GetOrdinal("AccessCompetencyId")),
=======
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    AccessCompetencyId = reader.GetInt64(reader.GetOrdinal("AccessCompetencyId")),
>>>>>>> Stashed changes
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                    LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                    LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
                };
            }

            return null;
        }

        public List<UserAccessCompetency> GetList()
        {
            var list = new List<UserAccessCompetency>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_UserAccessCompetency_GetList", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var entry = new UserAccessCompetency
                {
<<<<<<< Updated upstream
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    AccessCompetencyId = reader.GetInt32(reader.GetOrdinal("AccessCompetencyId")),
=======
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                    AccessCompetencyId = reader.GetInt64(reader.GetOrdinal("AccessCompetencyId")),
>>>>>>> Stashed changes
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                    LastEditAt = reader.IsDBNull(reader.GetOrdinal("LastEditAt")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastEditAt")),
                    LastEditBy = reader.IsDBNull(reader.GetOrdinal("LastEditBy")) ? null : reader.GetString(reader.GetOrdinal("LastEditBy"))
                };

                list.Add(entry);
            }

            return list;
        }

<<<<<<< Updated upstream
        public void Del(int id)
=======
        public void Del(long id)
>>>>>>> Stashed changes
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_UserAccessCompetency_Del", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UAC_ID", id);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
<<<<<<< Updated upstream

        public bool HasAccessCompetency(int userId, string competencyName)
=======
        public bool HasAccessCompetency(long userId, string competencyName)
>>>>>>> Stashed changes
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM USER_ACCESS_COMPETENCY uac
        INNER JOIN ACCESS_COMPETENCY ac ON ac.id = uac.AccessCompetencyId
        WHERE uac.UserId = @UserId
          AND ac.Name = @CompetencyName
          AND ac.IsActive = 1", conn);

            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@CompetencyName", competencyName);

            conn.Open();
            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

    }
}
