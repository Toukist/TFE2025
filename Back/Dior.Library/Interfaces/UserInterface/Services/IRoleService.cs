using Dior.Library.DTO.Role;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDefinitionDto>> GetAllAsync();
        Task<RoleDefinitionDto?> GetByIdAsync(long id);
        Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createDto);
        Task<bool> UpdateAsync(long id, UpdateRoleDefinitionDto updateDto);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<List<string>> GetRoleNamesAsync();
        Task<List<PrivilegeDto>> GetRolePrivilegesAsync(long roleId);
    }
}