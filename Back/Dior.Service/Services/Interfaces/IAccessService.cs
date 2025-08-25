using Dior.Data.DTO.Access;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    /// <summary>
    /// Service d'accès - opérations CRUD
    /// </summary>
    public interface IAccessService
    {
        /// <summary>Retourne tous les accès</summary>
        Task<IEnumerable<AccessDto>> GetAllAsync();
<<<<<<< Updated upstream
        Task<AccessDto?> GetByIdAsync(int id);
        Task<AccessDto> CreateAsync(CreateAccessDto createDto);
        Task<bool> UpdateAsync(int id, UpdateAccessDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
        Task<bool> DisableUserBadgeAsync(int userId, CancellationToken ct = default);
=======
        /// <summary>Retourne un accès par son identifiant</summary>
        Task<AccessDto?> GetByIdAsync(long id);
        /// <summary>Crée un accès</summary>
        Task<AccessDto> CreateAsync(CreateAccessDto createAccessDto);
        /// <summary>Met à jour un accès</summary>
        Task<bool> UpdateAsync(long id, UpdateAccessDto updateAccessDto);
        /// <summary>Supprime un accès</summary>
        Task<bool> DeleteAsync(long id);
>>>>>>> Stashed changes
    }
}
