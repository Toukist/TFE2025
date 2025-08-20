using Dior.Library.BO.UserInterface;

namespace Dior.Library.Interfaces.UserInterface.Services {
    public interface IUserRoleService {
        List<UserRole> GetList();
        UserRole Get(long id);
        long Add(UserRole entity, string editBy);
        void Set(UserRole entity, string editBy);
        void Del(long id);
    }
}
