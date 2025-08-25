using Dior.Library.Interfaces.UserInterface.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Services.Interfaces
{
    /// <summary>
    /// Service de gestion des UserAccess
    /// </summary>
    public interface IUserAccessService
    {
        Task<IEnumerable<UserAccessDto>> GetAllAsync();
        Task<UserAccessDto> GetByIdAsync(long id);
        Task<IEnumerable<UserAccessDto>> GetByUserIdAsync(long userId);
        Task<UserAccessDto> CreateAsync(CreateUserAccessDto createUserAccessDto);
        Task<bool> UpdateAsync(long id, UpdateUserAccessDto updateUserAccessDto);
        Task<bool> DeleteAsync(long id);
    }
}