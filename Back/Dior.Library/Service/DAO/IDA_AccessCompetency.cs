using Dior.Library.BO.UserInterface;

namespace Dior.Library.Service.DAO
{
    public interface IDA_AccessCompetency
    {
        AccessCompetency Get(int id);
        List<AccessCompetency> GetList();
        long Add(AccessCompetency accessCompetency, string editBy);
        void Set(AccessCompetency accessCompetency, string editBy);
        void Del(int id);
    }
}