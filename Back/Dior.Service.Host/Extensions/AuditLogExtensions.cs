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
                UserId = log.UserId ?? 0, // Correction CS0266 et CS8629 : utilise 0 si UserId est null
                UserName = log.User?.Name,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = log.RecordId ?? 0, // Correction similaire pour RecordId qui est aussi nullable
                Details = log.Details,
                Timestamp = log.Timestamp
            };
        }
    }
}
