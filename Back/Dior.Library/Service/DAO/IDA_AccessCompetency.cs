using Dior.Library.BO.UserInterface;

namespace Dior.Library.Service.DAO
{
    public interface IDA_AccessCompetency
    {
        long Add(AccessCompetency accessCompetency, string editBy);
        void Set(AccessCompetency accessCompetency, string editBy);
        void Del(long id);
        AccessCompetency Get(long id);
        List<AccessCompetency> GetList();
    }
}