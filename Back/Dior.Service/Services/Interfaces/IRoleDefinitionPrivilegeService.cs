using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.DTO 
{
    public interface IRoleDefinitionPrivilegeService
    {
        Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetAllAsync();
        Task<RoleDefinitionPrivilegeDto> GetByIdAsync(int id);
        Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetByRoleDefinitionIdAsync(int roleDefinitionId);
        Task<RoleDefinitionPrivilegeDto> CreateAsync(CreateRoleDefinitionPrivilegeDto createDto);
        Task<bool> UpdateAsync(int id, UpdateRoleDefinitionPrivilegeDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}