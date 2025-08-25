
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    /// <summary>
    /// Service de gestion des privil�ges
    /// </summary>
    public interface IPrivilegeService
    {
        /// <summary>Retourne tous les privil�ges</summary>
        Task<IEnumerable<PrivilegeDto>> GetAllAsync();
        /// <summary>Retourne un privil�ge par id</summary>
        Task<PrivilegeDto> GetByIdAsync(long id);
        /// <summary>Cr�e un privil�ge</summary>
        Task<PrivilegeDto> CreateAsync(CreatePrivilegeDto createPrivilegeDto);
        /// <summary>Met � jour un privil�ge</summary>
        Task<bool> UpdateAsync(long id, UpdatePrivilegeDto updatePrivilegeDto);
        /// <summary>Supprime un privil�ge</summary>
        Task<bool> DeleteAsync(long id);
    }
}