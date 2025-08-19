namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IPrivilegeService
    {
        Privilege Get(long id);
        List<Privilege> GetList();
        long Add(Privilege privilege, string editBy);
        void Set(Privilege privilege, string editBy);
        void Del(long id);
    }
}
