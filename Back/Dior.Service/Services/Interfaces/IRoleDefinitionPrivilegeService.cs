<<<<<<< Updated upstream
using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.DTO 
=======
using Dior.Library.DTO.Role;

namespace Dior.Service.Services.Interfaces
>>>>>>> Stashed changes
{
    /// <summary>
    /// Service de gestion des liaisons Rôle-Privilege
    /// </summary>
    public interface IRoleDefinitionPrivilegeService
    {
        Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetAllAsync();
        Task<RoleDefinitionPrivilegeDto?> GetByIdAsync(long id);
        Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetByRoleDefinitionIdAsync(long roleDefinitionId);
        Task<RoleDefinitionPrivilegeDto> CreateAsync(CreateRoleDefinitionPrivilegeDto createDto);
        Task<bool> UpdateAsync(long id, UpdateRoleDefinitionPrivilegeDto updateDto);
        Task<bool> DeleteAsync(long id);
    }
}