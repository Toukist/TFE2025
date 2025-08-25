using Dior.Library.DTO.Audit;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class AuditLogService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(DiorDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AuditLogDto>> GetLogsAsync(DateTime? from, DateTime? to)
        {
            var query = _context.AuditLogs.Include(al => al.User).AsQueryable();

            if (from.HasValue)
                query = query.Where(al => al.Timestamp >= from.Value);
            if (to.HasValue)
                query = query.Where(al => al.Timestamp <= to.Value);

            var logs = await query
                .OrderByDescending(al => al.Timestamp)
                .Take(100)
                .ToListAsync();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetUserLogsAsync(long userId)
        {
            var logs = await _context.AuditLogs
                .Where(al => al.UserId == userId)
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(50)
                .ToListAsync();

            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> SearchLogsAsync(string action, string table)
        {
            var query = _context.AuditLogs.Include(al => al.User).AsQueryable();

            if (!string.IsNullOrEmpty(action))
                query = query.Where(al => al.Action.Contains(action));
            if (!string.IsNullOrEmpty(table))
                query = query.Where(al => al.TableName.Contains(table));

            var logs = await query
                .OrderByDescending(al => al.Timestamp)
                .Take(100)
                .ToListAsync();

            return logs.Select(MapToDto).ToList();
        }

        private static AuditLogDto MapToDto(Dior.Library.Entities.AuditLog auditLog)
        {
            return new AuditLogDto
            {
                Id = auditLog.Id,
                Action = auditLog.Action,
                TableName = auditLog.TableName,
                RecordId = auditLog.RecordId,
                Details = auditLog.Details,
                Timestamp = auditLog.Timestamp,
                UserId = auditLog.UserId,
                UserName = auditLog.User?.UserName
            };
        }
    }
}
