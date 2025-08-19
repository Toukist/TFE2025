using Dior.Library.BO;
using Dior.Library.DTO;
using Dior.Service.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dior.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly string _connectionString;
        
        public MessageService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Dior_DB") 
                ?? configuration.GetConnectionString("DIOR_DB")
                ?? throw new ArgumentException("Connection string manquante");
        }
        
        public async Task<MessageDto> SendToTeamAsync(long senderId, CreateMessageRequest request)
        {
            if (!request.RecipientTeamId.HasValue)
                throw new ArgumentException("RecipientTeamId requis pour message d'équipe");
                
            var message = new Message
            {
                SenderId = senderId,
                RecipientTeamId = request.RecipientTeamId,
                Subject = request.Subject,
                Content = request.Content,
                Priority = request.Priority,
                MessageType = request.MessageType,
                SentAt = DateTime.Now
            };
            
            await InsertMessageAsync(message);
            return await MapToDto(message);
        }
        
        public async Task<MessageDto> SendToUserAsync(long senderId, CreateMessageRequest request)
        {
            if (!request.RecipientUserId.HasValue)
                throw new ArgumentException("RecipientUserId requis pour message individuel");
                
            var message = new Message
            {
                SenderId = senderId,
                RecipientUserId = request.RecipientUserId,
                Subject = request.Subject,
                Content = request.Content,
                Priority = request.Priority,
                MessageType = request.MessageType,
                SentAt = DateTime.Now
            };
            
            await InsertMessageAsync(message);
            return await MapToDto(message);
        }
        
        public async Task<List<MessageDto>> GetUserMessagesAsync(long userId)
        {
            var messages = new List<MessageDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT m.*, 
                       sender.FirstName + ' ' + sender.LastName as SenderName,
                       sender_role.Name as SenderRole,
                       recipient.FirstName + ' ' + recipient.LastName as RecipientUserName,
                       t.Name as RecipientTeamName
                FROM Messages m
                LEFT JOIN [USER] sender ON m.SenderId = sender.ID
                LEFT JOIN UserRole ur ON sender.ID = ur.UserId
                LEFT JOIN RoleDefinition sender_role ON ur.RoleDefinitionId = sender_role.Id
                LEFT JOIN [USER] recipient ON m.RecipientUserId = recipient.ID  
                LEFT JOIN Team t ON m.RecipientTeamId = t.Id
                WHERE m.RecipientUserId = @UserId 
                   OR m.RecipientTeamId IN (SELECT TeamId FROM [USER] WHERE ID = @UserId)
                ORDER BY m.SentAt DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(MapFromReader(reader));
            }
            
            return messages;
        }
        
        public async Task<List<MessageDto>> GetTeamMessagesAsync(int teamId)
        {
            var messages = new List<MessageDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT m.*, 
                       sender.FirstName + ' ' + sender.LastName as SenderName,
                       sender_role.Name as SenderRole,
                       t.Name as RecipientTeamName
                FROM Messages m
                LEFT JOIN [USER] sender ON m.SenderId = sender.ID
                LEFT JOIN UserRole ur ON sender.ID = ur.UserId
                LEFT JOIN RoleDefinition sender_role ON ur.RoleDefinitionId = sender_role.Id
                LEFT JOIN Team t ON m.RecipientTeamId = t.Id
                WHERE m.RecipientTeamId = @TeamId
                ORDER BY m.SentAt DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TeamId", teamId);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(MapFromReader(reader));
            }
            
            return messages;
        }
        
        public async Task<bool> MarkAsReadAsync(long messageId)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = "UPDATE Messages SET IsRead = 1, ReadAt = GETDATE() WHERE Id = @MessageId";
            
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MessageId", messageId);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteAsync(long messageId)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = "DELETE FROM Messages WHERE Id = @MessageId";
            
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MessageId", messageId);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        public async Task<int> GetUnreadCountAsync(long userId)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT COUNT(*) 
                FROM Messages 
                WHERE IsRead = 0 
                  AND (RecipientUserId = @UserId 
                       OR RecipientTeamId IN (SELECT TeamId FROM [USER] WHERE ID = @UserId))";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        
        private async Task InsertMessageAsync(Message message)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                INSERT INTO Messages (SenderId, RecipientUserId, RecipientTeamId, Subject, Content, Priority, MessageType, IsRead, SentAt)
                OUTPUT INSERTED.Id
                VALUES (@SenderId, @RecipientUserId, @RecipientTeamId, @Subject, @Content, @Priority, @MessageType, 0, @SentAt)";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SenderId", message.SenderId);
            cmd.Parameters.AddWithValue("@RecipientUserId", (object?)message.RecipientUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RecipientTeamId", (object?)message.RecipientTeamId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Subject", message.Subject);
            cmd.Parameters.AddWithValue("@Content", message.Content);
            cmd.Parameters.AddWithValue("@Priority", message.Priority);
            cmd.Parameters.AddWithValue("@MessageType", message.MessageType);
            cmd.Parameters.AddWithValue("@SentAt", message.SentAt);
            
            var newId = await cmd.ExecuteScalarAsync();
            message.Id = Convert.ToInt64(newId);
        }
        
        private async Task<MessageDto> MapToDto(Message message)
        {
            // TODO: Récupérer les noms du sender et recipient depuis la base
            return new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                RecipientUserId = message.RecipientUserId,
                RecipientTeamId = message.RecipientTeamId,
                Subject = message.Subject,
                Content = message.Content,
                Priority = message.Priority,
                MessageType = message.MessageType,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                SentAt = message.SentAt
            };
        }
        
        private MessageDto MapFromReader(SqlDataReader reader)
        {
            return new MessageDto
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                SenderId = reader.GetInt64(reader.GetOrdinal("SenderId")),
                SenderName = SafeGetString(reader, "SenderName") ?? string.Empty,
                SenderRole = SafeGetString(reader, "SenderRole") ?? string.Empty,
                RecipientUserId = SafeGetLong(reader, "RecipientUserId"),
                RecipientUserName = SafeGetString(reader, "RecipientUserName"),
                RecipientTeamId = SafeGetInt(reader, "RecipientTeamId"),
                RecipientTeamName = SafeGetString(reader, "RecipientTeamName"),
                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                MessageType = reader.GetString(reader.GetOrdinal("MessageType")),
                IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead")),
                ReadAt = SafeGetDateTime(reader, "ReadAt"),
                SentAt = reader.GetDateTime(reader.GetOrdinal("SentAt"))
            };
        }

        private string? SafeGetString(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        private DateTime? SafeGetDateTime(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        private long? SafeGetLong(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetInt64(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        private int? SafeGetInt(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
            }
            catch
            {
                return null;
            }
        }
    }
}