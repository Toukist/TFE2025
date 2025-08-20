using Dior.Library.BO.UserInterface;

namespace Dior.Library.Service.DAO
{
    public interface IDA_Privilege
    {
        Privilege Get(int id);
        List<Privilege> GetList();
        long Add(Privilege privilege, string editBy);
        void Set(Privilege privilege, string editBy);
        void Del(int id);
        long Add(Entities.Privilege privilege, string editBy);
        void Set(Entities.Privilege privilege, string editBy);
    }
}