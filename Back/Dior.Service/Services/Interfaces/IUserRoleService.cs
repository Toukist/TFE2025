using Dior.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleDto>> GetAllAsync();
        Task<UserRoleDto> GetByIdAsync(int id);
        Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(int userId);
        Task<UserRoleDto> CreateAsync(CreateUserRoleDto createUserRoleDto);
        Task<bool> UpdateAsync(int id, UpdateUserRoleDto updateUserRoleDto);
        Task<bool> DeleteAsync(int id);
    }
}