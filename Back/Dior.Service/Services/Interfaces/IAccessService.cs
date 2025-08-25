using Dior.Data.DTO.Access;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAccessService
    {
        Task<IEnumerable<AccessDto>> GetAllAsync();
        Task<AccessDto?> GetByIdAsync(int id);
        Task<AccessDto> CreateAsync(CreateAccessDto createDto);
        Task<bool> UpdateAsync(int id, UpdateAccessDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
        Task<bool> DisableUserBadgeAsync(int userId, CancellationToken ct = default);
    }
}
