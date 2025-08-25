using Dior.Library.DTO.Audit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<AuditLogDto?> GetByIdAsync(long id);
        Task<AuditLogDto> CreateAsync(CreateAuditLogDto createDto);
        Task<List<AuditLogDto>> GetByEntityAsync(string entityType, long entityId);
        Task<List<AuditLogDto>> GetByUserAsync(long userId);
        Task<List<AuditLogDto>> GetRecentAsync(int count = 50);
    }
}
