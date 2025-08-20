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
            _context = context;
        }

        // ------------------ MÉTHODES SYNCHRONES (AuditLog BO) ------------------

        public List<AuditLog> GetList()
        {
            var entityAuditLogs = _context.AuditLogs
                .OrderByDescending(al => al.CreatedAt)
                .ToList();

            return entityAuditLogs.Select(al => new AuditLog
            {
                Id = al.Id,
                UserId = al.UserId ?? 0,
                Action = al.Operation,
                TableName = al.TableName,
                RecordId = al.RecordId ?? 0,
                Details = CombineDetails(al.OldValues, al.NewValues),
                Timestamp = al.CreatedAt
            }).ToList();
        }

        public AuditLog Get(long id)
        {
            var entityAuditLog = _context.AuditLogs.Find((int)id);
            if (entityAuditLog == null) return null;

            return new AuditLog
            {
                Id = entityAuditLog.Id,
                UserId = entityAuditLog.UserId ?? 0,
                Action = entityAuditLog.Operation,
                TableName = entityAuditLog.TableName,
                RecordId = entityAuditLog.RecordId ?? 0,
                Details = CombineDetails(entityAuditLog.OldValues, entityAuditLog.NewValues),
                Timestamp = entityAuditLog.CreatedAt
            };
        }

        public long Add(AuditLog log, string editBy)
        {
            var entityAuditLog = new EntityAuditLog
            {
                UserId = (int)log.UserId,
                Operation = log.Action,
                TableName = log.TableName,
                RecordId = (int)log.RecordId,
                NewValues = log.Details,
                CreatedAt = DateTime.Now,
                CreatedBy = editBy
            };

            _context.AuditLogs.Add(entityAuditLog);
            _context.SaveChanges();

            return entityAuditLog.Id;
        }

        public void Set(AuditLog log, string editBy)
        {
            var entityAuditLog = _context.AuditLogs.Find((int)log.Id);
            if (entityAuditLog != null)
            {
                entityAuditLog.UserId = (int)log.UserId;
                entityAuditLog.Operation = log.Action;
                entityAuditLog.TableName = log.TableName;
                entityAuditLog.RecordId = (int)log.RecordId;
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
                .ToListAsync();
        }

        public async Task<EntityAuditLog?> GetByIdAsync(int id)
        {
            return await _context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<EntityAuditLog>> SearchAsync(int? userId, string? action)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Operation.Contains(action));

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        private string CombineDetails(string? oldValues, string? newValues)
        {
            if (!string.IsNullOrEmpty(oldValues) && !string.IsNullOrEmpty(newValues))
                return $"Old: {oldValues}, New: {newValues}";
            
            return newValues ?? oldValues ?? string.Empty;
        }
    }
}
