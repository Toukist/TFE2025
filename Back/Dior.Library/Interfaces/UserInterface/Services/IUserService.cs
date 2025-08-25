using Dior.Library.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dior.Library.DTO.Access;
namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto> CreateAsync(CreateUserDto createUserDto);
        Task<bool> UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> AuthenticateAsync(string email, string password);
        Task<List<UserFullDto>> GetFullUsersAsync();
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<List<AccessCompetencyDto>> GetUserAccessCompetenciesAsync(int userId);
    }
}