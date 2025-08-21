using Dior.Library.DTOs;
using Dior.Library.Entities;

namespace Dior.Library.DAO
{
    public interface IUserDao
    {
        int Add(Entities.User user, string editBy);
        void Set(Entities.User user, string editBy);
        List<Entities.User> GetList(List<int> userIds);
        List<Entities.User> GetList();
        Entities.User Get(int id);
        void Del(int id);
        List<UserDto> GetAllWithTeam();
        List<string> GetUserRoles(long userId);
        List<UserFullDto> GetUsersWithRoles();
    }
}
