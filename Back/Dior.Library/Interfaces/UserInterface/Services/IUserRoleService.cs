using Dior.Library.Entities;

namespace Dior.Library.Interfaces.UserInterface.Services {
    public interface IUserRoleService {
        List<Dior.Library.Entities.UserRole> GetList();
        Dior.Library.Entities.UserRole Get(long id);
        long Add(Dior.Library.Entities.UserRole entity, string editBy);
        void Set(Dior.Library.Entities.UserRole entity, string editBy);
        void Del(long id);
    }
}
