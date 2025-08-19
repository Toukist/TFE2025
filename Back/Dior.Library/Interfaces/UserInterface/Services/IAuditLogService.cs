namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAuditLogService
    {
        // Méthodes synchrones (BO)
        List<BO.UserInterface.AuditLog> GetList();
        BO.UserInterface.AuditLog Get(long id);
        long Add(BO.UserInterface.AuditLog log, string editBy);
        void Set(BO.UserInterface.AuditLog log, string editBy);
        void Del(long id);

        // Méthodes asynchrones (Entities)
        Task<List<Entities.AuditLog>> GetAllAsync();
        Task<Entities.AuditLog?> GetByIdAsync(int id);
        Task<List<Entities.AuditLog>> SearchAsync(int? userId, string? action);
    }
}
