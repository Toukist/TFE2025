using Dior.Library.DTO.Audit;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Service.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Service.Services.Implementations
{
    /// <summary>
    /// Service d'audit (lecture/écriture)
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        /// <summary>Constructor</summary>
        public AuditLogService(DiorDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Retourne tous les logs</summary>
        public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
        {
            var logs = await _context.AuditLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
            
            return logs.Select(MapToDto);
        }

        /// <summary>Retourne un log par id</summary>
        public async Task<AuditLogDto?> GetByIdAsync(long id)
        {
            var log = await _context.AuditLogs
                .Include(al => al.User)
                .FirstOrDefaultAsync(al => al.Id == id);
            
            return log != null ? MapToDto(log) : null;
        }

        /// <summary>Crée un log</summary>
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

        /// <summary>Retourne les logs d'un utilisateur</summary>
        public async Task<List<AuditLogDto>> GetByUserAsync(long userId)
        {
            var logs = await _context.AuditLogs
                .Where(al => al.UserId == userId)
                .Include(al => al.User)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
            
            return logs.Select(MapToDto).ToList();
        }

        /// <summary>Retourne les logs récents</summary>
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

        /// <summary>Non implémenté</summary>
        public Task<List<AuditLogDto>> GetByEntityAsync(string entityType, long entityId)
        {
            throw new NotImplementedException();
        }

        public async Task GetLogsAsync(DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}