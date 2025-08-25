using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    public interface IRoleDefinitionService
    {
        Task<IEnumerable<RoleDefinitionDto>> GetAllAsync();
        Task<RoleDefinitionDto> GetByIdAsync(int id);
        Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createRoleDefinitionDto);
        Task<bool> UpdateAsync(int id, UpdateRoleDefinitionDto updateRoleDefinitionDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<RoleDefinitionDto>> GetChildRolesAsync(int parentId);
    }
}