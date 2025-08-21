using Dior.Library.DTOs;
using Dior.Library.Entities;

namespace Dior.Library.Interfaces.Services 
{
    public interface IUserService 
    {
        User Authenticate(string username, string password);
        List<RoleDefinitionDto> GetUserRoles(long userId);
        List<PrivilegeDto> GetUserPrivileges(long userId);
        User GetUserById(long userId);
        IEnumerable<User> GetAllUsers();
        void CreateUser(User user, string password);
        void UpdateUser(User user);
        void DeleteUser(long userId);
    }
}