using Dior.Library.BO;
using Dior.Library.DTO;

namespace Dior.Service.Services
{
    public interface IProjetService
    {
        Task<List<ProjetDto>> GetAllAsync();
        Task<ProjetDto?> GetByIdAsync(int id);
        Task<List<ProjetDto>> GetByTeamIdAsync(int teamId);
        Task<List<ProjetDto>> GetByManagerIdAsync(int managerId);
        Task<ProjetDto> CreateAsync(CreateProjetRequest request, string createdBy);
        Task<bool> UpdateAsync(int id, UpdateProjetRequest request, string lastEditBy);
        Task<bool> DeleteAsync(int id);
    }
}