// Dior.Library\Service\DAO\IDA_User.cs

namespace Dior.Library.Service.DAO
{
    public interface IDA_User
    {
        List<User> GetList(List<int> userIds); // Un seul type d'identifiant
        User Get(int id);
        int Add(User entity, string editBy);
        void Set(User entity, string editBy);
        void Del(int id);
    }
}