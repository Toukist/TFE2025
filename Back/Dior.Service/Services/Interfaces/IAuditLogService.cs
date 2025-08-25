using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<AuditLogDto> GetByIdAsync(int id);
        Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AuditLogDto>> GetByTableNameAsync(string tableName);
        Task<AuditLogDto> CreateAsync(CreateAuditLogDto createAuditLogDto);
    }
}