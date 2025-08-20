

using Dior.Library.BO.UserInterface;

namespace Dior.Library.Service.DAO
{
    public interface IDA_Privilege
    {
        long Add(Privilege privilege, string editBy);
        void Set(Privilege privilege, string editBy);
        void Del(long id);
        Privilege Get(long id);
        List<Privilege> GetList();
    }
}