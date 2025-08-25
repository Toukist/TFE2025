using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    /// <summary>
    /// Service de gestion des définitions de rôles
    /// </summary>
    public interface IRoleDefinitionService
    {
        Task<IEnumerable<RoleDefinitionDto>> GetAllAsync();
        Task<RoleDefinitionDto> GetByIdAsync(long id);
        Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createRoleDefinitionDto);
        Task<bool> UpdateAsync(long id, UpdateRoleDefinitionDto updateRoleDefinitionDto);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<RoleDefinitionDto>> GetChildRolesAsync(long parentId);
    }
}