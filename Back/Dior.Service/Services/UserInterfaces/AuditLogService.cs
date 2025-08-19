using Dior.Library.Interfaces.UserInterface.Services;
using Microsoft.EntityFrameworkCore;

// Aliases pour éviter les conflits de noms
using BOAuditLog = Dior.Library.BO.UserInterface.AuditLog;
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

        // ------------------ MÉTHODES SYNCHRONES (BOAuditLog) ------------------

        public List<BOAuditLog> GetList()
        {
            var entityAuditLogs = _context.AuditLogs
                .OrderByDescending(al => al.Timestamp)
                .ToList();

            return entityAuditLogs.Select(al => new BOAuditLog
            {
                Id = al.Id,
                UserId = al.UserId,
                Action = al.Action,
                TableName = al.TableName,
                RecordId = al.RecordId,
                Details = al.Details,
                Timestamp = al.Timestamp
            }).ToList();
        }

        public BOAuditLog Get(long id)
        {
            var entityAuditLog = _context.AuditLogs.Find((int)id);
            if (entityAuditLog == null) return null;

            return new BOAuditLog
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

        public long Add(BOAuditLog log, string editBy)
        {
            var entityAuditLog = new EntityAuditLog
            {
                UserId = (int)log.UserId,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = (int)log.RecordId,
                Details = log.Details,
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(entityAuditLog);
            _context.SaveChanges();

            return entityAuditLog.Id;
        }

        public void Set(BOAuditLog log, string editBy)
        {
            var entityAuditLog = _context.AuditLogs.Find((int)log.Id);
            if (entityAuditLog != null)
            {
                entityAuditLog.UserId = (int)log.UserId;
                entityAuditLog.Action = log.Action;
                entityAuditLog.TableName = log.TableName;
                entityAuditLog.RecordId = (int)log.RecordId;
                entityAuditLog.Details = log.Details;
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

        // Remplacer les méthodes asynchrones pour qu'elles correspondent à la signature de l'interface
        public async Task<List<EntityAuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
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
                query = query.Where(a => a.Action.Contains(action));

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }


    }
}
