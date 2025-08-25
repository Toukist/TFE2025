
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    /// <summary>
    /// Service de gestion des privilèges
    /// </summary>
    public interface IPrivilegeService
    {
        /// <summary>Retourne tous les privilèges</summary>
        Task<IEnumerable<PrivilegeDto>> GetAllAsync();
        /// <summary>Retourne un privilège par id</summary>
        Task<PrivilegeDto> GetByIdAsync(long id);
        /// <summary>Crée un privilège</summary>
        Task<PrivilegeDto> CreateAsync(CreatePrivilegeDto createPrivilegeDto);
        /// <summary>Met à jour un privilège</summary>
        Task<bool> UpdateAsync(long id, UpdatePrivilegeDto updatePrivilegeDto);
        /// <summary>Supprime un privilège</summary>
        Task<bool> DeleteAsync(long id);
    }
}