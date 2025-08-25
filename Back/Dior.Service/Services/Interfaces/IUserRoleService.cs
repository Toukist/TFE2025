<<<<<<< Updated upstream
using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
=======
using Dior.Library.DTO.Role;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Service.Services.Interfaces
>>>>>>> Stashed changes
{
    /// <summary>
    /// Contrat du service UserRole
    /// </summary>
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleDto>> GetAllAsync();
        Task<UserRoleDto?> GetByIdAsync(long id);
        Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(long userId);
        Task<UserRoleDto> CreateAsync(CreateUserRoleDto createUserRoleDto);
        Task<bool> UpdateAsync(long id, UpdateUserRoleDto updateUserRoleDto);
        Task<bool> DeleteAsync(long id);
    }
}