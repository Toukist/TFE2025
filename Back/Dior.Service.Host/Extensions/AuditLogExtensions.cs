using Dior.Library.DTO;
using Dior.Library.Entities;

namespace Dior.Service.Host.Extensions
{
    public static class AuditLogExtensions
    {
        public static AuditLogDto ToDto(this AuditLog log)
        {
            return new AuditLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                Username = log.User?.Username,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = log.RecordId,
                Details = log.Details,
                Timestamp = log.Timestamp
            };
        }
    }
}
