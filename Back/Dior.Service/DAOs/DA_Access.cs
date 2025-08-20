using Dior.Library.Interfaces.DAOs;
using Dior.Library.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAOs
{
    public class DA_Access : IDA_Access
    {
        private readonly string _connectionString;

        public DA_Access(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey) 
                ?? throw new InvalidOperationException("Database connection string not found");
        }

        public Access? GetAccessById(int id)
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
                    return MapAccess(reader);
                }
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
            return null;
        }

        public IEnumerable<Access> GetAllAccesses()
        {
            var accesses = new List<Access>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT * FROM Access", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    accesses.Add(MapAccess(reader));
                }
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
            return accesses;
        }

        public void CreateAccess(Access access)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_CreateAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Name", access.Name ?? string.Empty);
                cmd.Parameters.AddWithValue("@Description", access.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", access.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", access.CreatedBy ?? string.Empty);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
        }

        public void UpdateAccess(Access access)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_UpdateAccess", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", access.Id);
                cmd.Parameters.AddWithValue("@Name", access.Name ?? string.Empty);
                cmd.Parameters.AddWithValue("@Description", access.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", access.IsActive);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: Use proper logging
                throw;
            }
        }

        public void DeleteAccess(int id)
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
                // TODO: Use proper logging
                throw;
            }
        }

        private static Access MapAccess(SqlDataReader reader)
        {
            return new Access
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                IsActive = !reader.IsDBNull(reader.GetOrdinal("IsActive")) && reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.UtcNow : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader["CreatedBy"]?.ToString() ?? string.Empty
            };
        }
    }
}