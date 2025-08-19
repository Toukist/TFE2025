using Dior.Library.DTO;

namespace Dior.Library.DAO
{
    public interface IUserDao
    {
        int Add(User user, string editBy);
        void Set(User user, string editBy);
        List<User> GetList(List<int> userIds);
        List<User> GetList();
        User Get(int id);
        void Del(int id);
        List<UserDto> GetAllWithTeam();
        List<string> GetUserRoles(long userId);

        List<UserFullDto> GetUsersWithRoles(); // pour table Angular avec rôles 
    }
}
