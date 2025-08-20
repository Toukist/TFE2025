using Dior.Library.BO.UserInterface;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAccessCompetencyService
    {
        List<AccessCompetency> GetList();
        AccessCompetency Get(long id);
        long Add(AccessCompetency accessCompetency, string editBy);
        void Set(AccessCompetency accessCompetency, string editBy);
        void Del(long id);
    }
}
