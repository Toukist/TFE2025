using Dior.Library.DTO.Audit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();
        Task<AuditLogDto?> GetByIdAsync(int id);
        Task<AuditLogDto> CreateAsync(CreateAuditLogDto createDto);
        Task<List<AuditLogDto>> GetByEntityAsync(string entityType, int entityId);
        Task<List<AuditLogDto>> GetByUserAsync(int userId);
        Task<List<AuditLogDto>> GetRecentAsync(int count = 50);
    }
}
