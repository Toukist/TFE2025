using Dior.Database.DTOs.UserAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Services.Interfaces
{
    public interface IUserAccessService
    {
        Task<IEnumerable<UserAccessDto>> GetAllAsync();
        Task<UserAccessDto> GetByIdAsync(int id);
        Task<IEnumerable<UserAccessDto>> GetByUserIdAsync(int userId);
        Task<UserAccessDto> CreateAsync(CreateUserAccessDto createUserAccessDto);
        Task<bool> UpdateAsync(int id, UpdateUserAccessDto updateUserAccessDto);
        Task<bool> DeleteAsync(int id);
    }
}