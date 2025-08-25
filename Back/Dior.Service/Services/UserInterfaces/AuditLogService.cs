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

        // ------------------ MÉTHODES SYNCHRONES ------------------

        public List<AuditLog> GetList()
        {
            var entityAuditLogs = _context.AuditLogs
                .OrderByDescending(al => al.Timestamp)
                .ToList();

            return entityAuditLogs.Select(MapToBusinessObject).ToList();
        }

        public AuditLog? Get(long id)
        {
            var entityAuditLog = _context.AuditLogs.Find(id);
            return entityAuditLog == null ? null : MapToBusinessObject(entityAuditLog);
        }

        public long Add(AuditLog log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            var entityAuditLog = new EntityAuditLog
            {
                UserId = log.UserId,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = (int)log.RecordId,
                Details = log.Details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(entityAuditLog);
            _context.SaveChanges();

            return entityAuditLog.Id;
        }

        public void Set(AuditLog log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            var entityAuditLog = _context.AuditLogs.Find(log.Id);
            if (entityAuditLog != null)
            {
                entityAuditLog.UserId = log.UserId;
                entityAuditLog.Action = log.Action;
                entityAuditLog.TableName = log.TableName;
                entityAuditLog.RecordId = (int)log.RecordId;
                entityAuditLog.Details = log.Details;
                entityAuditLog.Timestamp = DateTime.UtcNow;

                _context.SaveChanges();
            }
        }

        public void Del(long id)
        {
            var entityAuditLog = _context.AuditLogs.Find(id);
            if (entityAuditLog != null)
            {
                _context.AuditLogs.Remove(entityAuditLog);
                _context.SaveChanges();
            }
        }

        // ------------------ MÉTHODES ASYNCHRONES ------------------

        public async Task<List<EntityAuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<EntityAuditLog?> GetByIdAsync(long id)
        {
            return await _context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == id)
                .ConfigureAwait(false);
        }

        public async Task<List<EntityAuditLog>> SearchAsync(long? userId, string? action)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action.Contains(action));

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        // ------------------ MAPPING ------------------

        private static AuditLog MapToBusinessObject(EntityAuditLog entityAuditLog)
        {
            return new AuditLog
            {
                Id = entityAuditLog.Id,
                UserId = entityAuditLog.UserId,
                Action = entityAuditLog.Action,
                TableName = entityAuditLog.TableName,
                RecordId = entityAuditLog.RecordId,
                Details = entityAuditLog.Details,
                Timestamp = entityAuditLog.Timestamp
            };
        }

        // ------------------ Méthodes IAuditLogService (pas encore implémentées) ------------------

        Task<IEnumerable<Dior.Library.DTO.Audit.AuditLogDto>> IAuditLogService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Dior.Library.DTO.Audit.AuditLogDto?> IAuditLogService.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<Dior.Library.DTO.Audit.AuditLogDto> IAuditLogService.CreateAsync(Dior.Library.DTO.Audit.CreateAuditLogDto createDto)
        {
            throw new NotImplementedException();
        }

        Task<List<Dior.Library.DTO.Audit.AuditLogDto>> IAuditLogService.GetByEntityAsync(string entityType, int entityId)
        {
            throw new NotImplementedException();
        }

        Task<List<Dior.Library.DTO.Audit.AuditLogDto>> IAuditLogService.GetByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task<List<Dior.Library.DTO.Audit.AuditLogDto>> IAuditLogService.GetRecentAsync(int count)
        {
            throw new NotImplementedException();
        }
    }
}
