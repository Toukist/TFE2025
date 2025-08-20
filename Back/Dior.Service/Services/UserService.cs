using Dior.Library.Interfaces.Services;
using Dior.Library.Interfaces.DAOs;
using Dior.Library.Entities;
using Dior.Library.DTO;

namespace Dior.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IDA_User _DA_User;

        public UserService(IDA_User daUser)
        {
            _DA_User = daUser ?? throw new ArgumentNullException(nameof(daUser));
        }

        public User? Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            if (string.IsNullOrWhiteSpace(password)) return null;

            var user = _DA_User.GetUserByUsername(username);
            if (user != null && user.IsActive)
            {
                // TODO: V�rifier le hash du mot de passe
                return user;
            }
            return null;
        }

        public List<RoleDefinitionDto> GetUserRoles(long userId)
        {
            // TODO: Impl�menter la r�cup�ration des r�les via DAO
            return new List<RoleDefinitionDto>();
        }

        public List<PrivilegeDto> GetUserPrivileges(long userId)
        {
            // TODO: Impl�menter la r�cup�ration des privil�ges via DAO
            return new List<PrivilegeDto>();
        }

        public User? GetUserById(long userId)
        {
            return _DA_User.GetUserById(userId);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _DA_User.GetAllUsers();
        }

        public void CreateUser(User user, string password)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // TODO: Hash the password
            user.PasswordHash = password; // Remplacer par BCrypt
            user.CreatedAt = DateTime.UtcNow;
            _DA_User.CreateUser(user);
        }

        public void UpdateUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.LastEditAt = DateTime.UtcNow;
            _DA_User.UpdateUser(user);
        }

        public void DeleteUser(long userId)
        {
            _DA_User.DeleteUser(userId);
        }
    }
}