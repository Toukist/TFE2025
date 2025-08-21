using Dior.Library.DTO.Role;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDefinitionDto>> GetAllAsync();
        Task<RoleDefinitionDto?> GetByIdAsync(int id);
        Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createDto);
        Task<bool> UpdateAsync(int id, UpdateRoleDefinitionDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<string>> GetRoleNamesAsync();
        Task<List<PrivilegeDto>> GetRolePrivilegesAsync(int roleId);
    }
}