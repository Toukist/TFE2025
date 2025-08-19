using Dior.Library.DTO;
using Dior.Library.Service.DAO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAO.UserInterfaces
{
    public class DA_Access : IDA_Access
    {
        private readonly string _connectionString;
        public DA_Access(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"] ?? "DIOR_DB";
            _connectionString = configuration.GetConnectionString(activeDbKey)
                ?? throw new InvalidOperationException($"Connection string '{activeDbKey}' not found");
        }
        public long Add(Access entity, string editBy)
        {
            throw new NotImplementedException();
        }

        public void Del(long id)
        {
            throw new NotImplementedException();
        }

        public Access Get(long id)
        {
            throw new NotImplementedException();
        }

        public List<AccessDto> GetList()
        {
            var list = new List<AccessDto>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_GetAccessList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AccessDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            BadgePhysicalNumber = reader.GetString(reader.GetOrdinal("badgePhysicalNumber")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("isActive")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("createdBy")) ? null : reader.GetString(reader.GetOrdinal("createdBy"))
                        });
                    }
                }
            }
            return list;
        }

        public void Set(Access entity, string editBy)
        {
            throw new NotImplementedException();
        }
    }
}
