using Dior.Library.Entities;
using Dior.Library.Service.DAO;
using Dior.Library.DTO;
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

        public Access Get(long id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapToEntity(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DA_Access.Get: {ex.Message}");
            }
            return null;
        }

        public List<AccessDto> GetList()
        {
            var list = new List<AccessDto>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetAccessList", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDto(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DA_Access.GetList: {ex.Message}");
            }
            return list;
        }

        public long Add(Access entity, string editBy)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_AddAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@BadgePhysicalNumber", entity.BadgePhysicalNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", editBy);

                var outputParam = new SqlParameter("@NewId", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                return (long)outputParam.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DA_Access.Add: {ex.Message}");
                return -1;
            }
        }

        public void Set(Access entity, string editBy)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_UpdateAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", entity.Id);
                cmd.Parameters.AddWithValue("@BadgePhysicalNumber", entity.BadgePhysicalNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
                cmd.Parameters.AddWithValue("@LastEditBy", editBy);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DA_Access.Set: {ex.Message}");
            }
        }

        public void Del(long id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_DeleteAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DA_Access.Del: {ex.Message}");
            }
        }

        private Access MapToEntity(SqlDataReader reader)
        {
            return new Access
            {
                Id = reader.GetInt32("Id"),
                BadgePhysicalNumber = reader.IsDBNull("BadgePhysicalNumber") ? null : reader.GetString("BadgePhysicalNumber"),
                IsActive = reader.GetBoolean("IsActive"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetString("CreatedBy"),
                LastEditAt = reader.IsDBNull("LastEditAt") ? null : reader.GetDateTime("LastEditAt"),
                LastEditBy = reader.IsDBNull("LastEditBy") ? null : reader.GetString("LastEditBy")
            };
        }

        private AccessDto MapToDto(SqlDataReader reader)
        {
            return new AccessDto
            {
                Id = reader.GetInt32("Id"),
                BadgePhysicalNumber = reader.IsDBNull("BadgePhysicalNumber") ? null : reader.GetString("BadgePhysicalNumber"),
                IsActive = reader.GetBoolean("IsActive"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetString("CreatedBy"),
                LastEditAt = reader.IsDBNull("LastEditAt") ? null : reader.GetDateTime("LastEditAt"),
                LastEditBy = reader.IsDBNull("LastEditBy") ? null : reader.GetString("LastEditBy")
            };
        }
    }
}
