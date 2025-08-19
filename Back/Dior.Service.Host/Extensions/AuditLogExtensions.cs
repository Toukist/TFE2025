using Dior.Library.Entities;
using Dior.Service.Host.Models;

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
                UserName = log.User?.Name,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = log.RecordId,
                Details = log.Details,
                Timestamp = log.Timestamp
            };
        }
    }
}
