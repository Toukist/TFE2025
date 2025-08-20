using Dior.Library.Interfaces;
using Dior.Library.Entities;
using Dior.Service.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Dior.Service.Services
{
    public class AuditService : IAuditService
    {
        private readonly DiorDbContext _context;
        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AuditService(DiorDbContext context)
        {
            _context = context;
        }

        public async Task WriteAsync(int userId, string action, string entity, int entityId, object? before, object? after, CancellationToken ct = default)
        {
            var oldValuesJson = before != null ? JsonSerializer.Serialize(before, _jsonOpts) : null;
            var newValuesJson = after != null ? JsonSerializer.Serialize(after, _jsonOpts) : null;

            var auditLog = new AuditLog
            {
                UserId = userId,
                Operation = action,
                TableName = entity,
                RecordId = entityId,
                OldValues = oldValuesJson,
                NewValues = newValuesJson,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId.ToString()
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(ct);
        }

        public async Task LogAsync(int userId, string action, string entity, int entityId, object? before = null, object? after = null)
        {
            await WriteAsync(userId, action, entity, entityId, before, after);
        }

        public async Task LogAsync(ClaimsPrincipal user, string action, string entity, int entityId, object? before = null, object? after = null)
        {
            var userIdClaim = user.FindFirst("userId")?.Value
                           ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                await LogAsync(userId, action, entity, entityId, before, after);
            }
        }

        public async Task LogCreateAsync(int userId, string entity, int entityId, object after)
        {
            await LogAsync(userId, "CREATE", entity, entityId, null, after);
        }

        public async Task LogUpdateAsync(int userId, string entity, int entityId, object before, object after)
        {
            await LogAsync(userId, "UPDATE", entity, entityId, before, after);
        }

        public async Task LogDeleteAsync(int userId, string entity, int entityId, object before)
        {
            await LogAsync(userId, "DELETE", entity, entityId, before, null);
        }
    }
}