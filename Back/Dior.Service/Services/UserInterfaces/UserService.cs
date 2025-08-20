using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.Entities;
using Dior.Library.DTO;

namespace Dior.Service.Services.UserInterfaces
{
    public class UserService : IUserService
    {
        private readonly IDA_User _DA_User;

        public UserService(IDA_User daUser)
        {
            _DA_User = daUser;
        }

        public User Authenticate(string username, string password)
        {
            // Implémentation basique - à améliorer avec hash password
            var user = _DA_User.GetUserByUsername(username);
            if (user != null && user.IsActive)
            {
                // TODO: Vérifier le hash du mot de passe
                return user;
            }
            return null;
        }

        public List<RoleDefinitionDto> GetUserRoles(long userId)
        {
            // TODO: Implémenter la récupération des rôles via DAO
            return new List<RoleDefinitionDto>();
        }

        public List<PrivilegeDto> GetUserPrivileges(long userId)
        {
            // TODO: Implémenter la récupération des privilèges via DAO
            return new List<PrivilegeDto>();
        }

        public User GetUserById(long userId)
        {
            return _DA_User.GetUserById(userId);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _DA_User.GetAllUsers();
        }

        public void CreateUser(User user, string password)
        {
            // TODO: Hash the password
            user.PasswordHash = password; // Remplacer par BCrypt
            _DA_User.CreateUser(user);
        }

        public void UpdateUser(User user)
        {
            _DA_User.UpdateUser(user);
        }

        public void DeleteUser(long userId)
        {
            _DA_User.DeleteUser(userId);
        }
    }
}
