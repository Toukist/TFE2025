using Dior.Library.Entities;

namespace Dior.Library.Interfaces.DAOs 
{
    public interface IDA_User 
    {
        User GetUserByUsername(string username);
        User GetUserById(long id);
        IEnumerable<User> GetAllUsers();
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(long id);
    }
}