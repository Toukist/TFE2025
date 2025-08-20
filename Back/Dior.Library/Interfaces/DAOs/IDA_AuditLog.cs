using Dior.Library.Entities;
namespace Dior.Library.Interfaces.DAOs {
    public interface IDA_AuditLog {
        AuditLog GetAuditLogById(int id);
        IEnumerable<AuditLog> GetAllAuditLogs();
        void CreateAuditLog(AuditLog auditLog);
        void UpdateAuditLog(AuditLog auditLog);
        void DeleteAuditLog(int id);
    }
}