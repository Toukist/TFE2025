using DiorUser = Dior.Library.BO.UserInterface.User;
using Dior.Library.Entities;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IUserService
    {
        List<DiorUser> GetList(List<long> userIDs = null);
        long Add(DiorUser user, string editBy);
        void Set(DiorUser user, string editBy);
        void Del(long id);
        DiorUser Get(long userId);
        object Add(User user, string editBy);
        void Set(User user, string editBy);
    }
}