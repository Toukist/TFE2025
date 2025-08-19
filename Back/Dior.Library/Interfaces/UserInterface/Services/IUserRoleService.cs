namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IUserRoleService
    {
        List<UserRole> GetList();
        UserRole Get(long id);
        long Add(UserRole role, string editBy);
        void Set(UserRole role, string editBy);
        void Del(long id);
    }
}
