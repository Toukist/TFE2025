using Dior.Library.DTO.Audit;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Service.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Service.Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(DiorDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
        {
            var logs = await _context.AuditLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
            
            return logs.Select(MapToDto);
        }

        public async Task<AuditLogDto?> GetByIdAsync(int id)
        {
            var log = await _context.AuditLogs
                .Include(al => al.User)
                .FirstOrDefaultAsync(al => al.Id == id);
            
            return log != null ? MapToDto(log) : null;
        }

        public async Task<AuditLogDto> CreateAsync(CreateAuditLogDto createDto)
        {
            var auditLog = new Dior.Library.Entities.AuditLog
            {
                Action = createDto.Action,
                OldValues = createDto.OldValues,
                NewValues = createDto.NewValues,
                Timestamp = DateTime.UtcNow,
                UserId = createDto.UserId,

            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            
            return MapToDto(auditLog);
        }

    

        public async Task<List<AuditLogDto>> GetByUserAsync(int userId)
        {
            var logs = await _context.AuditLogs
                .Where(al => al.UserId == userId)
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
            
            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetRecentAsync(int count = 50)
        {
            var logs = await _context.AuditLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .Take(count)
                .ToListAsync();
            
            return logs.Select(MapToDto).ToList();
        }

        private static AuditLogDto MapToDto(Dior.Library.Entities.AuditLog auditLog)
        {
            return new AuditLogDto
            {
                Id = auditLog.Id,
                Action = auditLog.Action,
                OldValues = auditLog.OldValues,
                NewValues = auditLog.NewValues,
                Timestamp = auditLog.Timestamp,
                UserName = auditLog.User?.UserName,
            };
        }

        public Task<List<AuditLogDto>> GetByEntityAsync(string entityType, int entityId)
        {
            throw new NotImplementedException();
        }

        public async Task GetLogsAsync(DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}