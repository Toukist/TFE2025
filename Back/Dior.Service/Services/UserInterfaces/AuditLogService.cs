using Dior.Library.Interfaces.UserInterface.Services;
using Microsoft.EntityFrameworkCore;
using Dior.Library.BO.UserInterface;
using EntityAuditLog = Dior.Library.Entities.AuditLog;

namespace Dior.Service.Services.UserInterfaces
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DiorDbContext _context;

        public AuditLogService(DiorDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // ------------------ MÉTHODES SYNCHRONES (AuditLog BO) ------------------

        public List<AuditLog> GetList()
        {
            var entityAuditLogs = _context.AuditLogs
                .OrderByDescending(al => al.CreatedAt)
                .ToList();

            return entityAuditLogs.Select(MapToBusinessObject).ToList();
        }

        public AuditLog? Get(long id)
        {
            var entityAuditLog = _context.AuditLogs.Find((int)id);
            return entityAuditLog == null ? null : MapToBusinessObject(entityAuditLog);
        }

        public long Add(AuditLog log, string editBy)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(editBy)) throw new ArgumentException("EditBy cannot be null or empty", nameof(editBy));

            var entityAuditLog = new EntityAuditLog
            {
                UserId = log.UserId > 0 ? (int)log.UserId : null,
                Action = log.Action ?? string.Empty,
                TableName = log.TableName ?? string.Empty,
                RecordId = log.RecordId > 0 ? (int)log.RecordId : null,
                NewValues = log.Details,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = editBy
            };

            _context.AuditLogs.Add(entityAuditLog);
            _context.SaveChanges();

            return entityAuditLog.Id;
        }

        public void Set(AuditLog log, string editBy)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(editBy)) throw new ArgumentException("EditBy cannot be null or empty", nameof(editBy));

            var entityAuditLog = _context.AuditLogs.Find((int)log.Id);
            if (entityAuditLog != null)
            {
                entityAuditLog.UserId = log.UserId > 0 ? (int)log.UserId : null;
                entityAuditLog.Action = log.Action ?? string.Empty;
                entityAuditLog.TableName = log.TableName ?? string.Empty;
                entityAuditLog.RecordId = log.RecordId > 0 ? (int)log.RecordId : null;
                entityAuditLog.NewValues = log.Details;
                _context.SaveChanges();
            }
        }

        public void Del(long id)
        {
            var entityAuditLog = _context.AuditLogs.Find((int)id);
            if (entityAuditLog != null)
            {
                _context.AuditLogs.Remove(entityAuditLog);
                _context.SaveChanges();
            }
        }

        // Méthodes asynchrones pour les entités
        public async Task<List<EntityAuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<EntityAuditLog?> GetByIdAsync(int id)
        {
            return await _context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == id)
                .ConfigureAwait(false);
        }

        public async Task<List<EntityAuditLog>> SearchAsync(int? userId, string? action)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action.Contains(action));

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        private static AuditLog MapToBusinessObject(EntityAuditLog entityAuditLog)
        {
            return new AuditLog
            {
                Id = entityAuditLog.Id,
                UserId = entityAuditLog.UserId ?? 0,
                Action = entityAuditLog.Action ?? string.Empty,
                TableName = entityAuditLog.TableName ?? string.Empty,
                RecordId = entityAuditLog.RecordId ?? 0,
                Details = CombineDetails(entityAuditLog.OldValues, entityAuditLog.NewValues),
                Timestamp = entityAuditLog.CreatedAt
            };
        }

        private static string CombineDetails(string? oldValues, string? newValues)
        {
            if (!string.IsNullOrEmpty(oldValues) && !string.IsNullOrEmpty(newValues))
                return $"Old: {oldValues}, New: {newValues}";
            
            return newValues ?? oldValues ?? string.Empty;
        }

        Task<IEnumerable<AuditLogDto>> IAuditLogService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<AuditLogDto?> IAuditLogService.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AuditLogDto> CreateAsync(CreateAuditLogDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLogDto>> GetByEntityAsync(string entityType, int entityId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLogDto>> GetByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AuditLogDto>> GetRecentAsync(int count = 50)
        {
            throw new NotImplementedException();
        }
    }
}
