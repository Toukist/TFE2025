using Dior.Library.Interfaces.DAOs;
using Dior.Library.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Dior.Service.DAOs
{
    public class DA_AuditLog : IDA_AuditLog
    {
        private readonly string _connectionString;

        public DA_AuditLog(IConfiguration configuration)
        {
            var activeDbKey = configuration["appSettings:ACTIVE_DB"];
            _connectionString = configuration.GetConnectionString(activeDbKey);
        }

        public AuditLog? GetAuditLogById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_GetAuditLog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return MapAuditLog(reader);
                }
            }
            catch
            {
                // log si besoin
            }
            return null;
        }

        public IEnumerable<AuditLog> GetAllAuditLogs()
        {
            var auditLogs = new List<AuditLog>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT * FROM AUDITLOGS", conn); // ✅ nom corrigé
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    auditLogs.Add(MapAuditLog(reader));
                }
            }
            catch
            {
                // log si besoin
            }
            return auditLogs;
        }

        public void CreateAuditLog(AuditLog auditLog)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_CreateAuditLog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", auditLog.UserId);
                cmd.Parameters.AddWithValue("@Action", auditLog.Action);
                cmd.Parameters.AddWithValue("@TableName", auditLog.TableName);
                cmd.Parameters.AddWithValue("@RecordId", auditLog.RecordId);
                cmd.Parameters.AddWithValue("@Details", (object?)auditLog.Details ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Timestamp", auditLog.Timestamp);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // log si besoin
            }
        }

        public void UpdateAuditLog(AuditLog auditLog)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_UpdateAuditLog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", auditLog.Id);
                cmd.Parameters.AddWithValue("@UserId", auditLog.UserId);
                cmd.Parameters.AddWithValue("@Action", auditLog.Action);
                cmd.Parameters.AddWithValue("@TableName", auditLog.TableName);
                cmd.Parameters.AddWithValue("@RecordId", auditLog.RecordId);
                cmd.Parameters.AddWithValue("@Details", (object?)auditLog.Details ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Timestamp", auditLog.Timestamp);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // log si besoin
            }
        }

        public void DeleteAuditLog(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("sp_DeleteAuditLog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // log si besoin
            }
        }

        private AuditLog MapAuditLog(SqlDataReader reader)
        {
            return new AuditLog
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                Action = reader["Action"]?.ToString() ?? string.Empty,
                TableName = reader["TableName"]?.ToString() ?? string.Empty,
                RecordId = reader.GetInt32(reader.GetOrdinal("RecordId")),
                Details = reader["Details"]?.ToString(),
                Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
            };
        }
    }
}
