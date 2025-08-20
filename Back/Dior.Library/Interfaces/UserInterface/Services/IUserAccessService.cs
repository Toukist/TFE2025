using Dior.Library.Entities;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IUserAccessService
    {
        List<UserAccess> GetList();
        UserAccess Get(long id);
        long Add(UserAccess userAccess, string editBy);
        void Set(UserAccess userAccess, string editBy);
        void Del(long id);
        int? GetActiveAccessIdByUserId(int userId);
    }
}
