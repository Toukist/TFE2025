using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Services.Interfaces
{
    /// <summary>
    /// Service de logs d'audit
    /// </summary>
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<AuditLogDto> GetByIdAsync(long id);
        Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(long userId);
        Task<IEnumerable<AuditLogDto>> GetByTableNameAsync(string tableName);
        Task<AuditLogDto> CreateAsync(CreateAuditLogDto createAuditLogDto);
    }
}