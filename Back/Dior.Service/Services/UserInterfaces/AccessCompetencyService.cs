using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.BO.UserInterface;

namespace Dior.Service.Services.UserInterfaces
{
    public class AccessCompetencyService : IAccessCompetencyService
    {
        private readonly IDA_AccessCompetency _DA_AccessCompetency;

        public AccessCompetencyService(IDA_AccessCompetency DA_AccessCompetency)
        {
            this._DA_AccessCompetency = DA_AccessCompetency;
        }

        public List<AccessCompetency> GetList()
        {
            return this._DA_AccessCompetency.GetList();
        }

        public AccessCompetency Get(long id)
        {
            return this._DA_AccessCompetency.Get(id);
        }

        public long Add(AccessCompetency accessCompetency, string editBy)
        {
            return this._DA_AccessCompetency.Add(accessCompetency, editBy);
        }

        public void Set(AccessCompetency accessCompetency, string editBy)
        {
            this._DA_AccessCompetency.Set(accessCompetency, editBy);
        }

        public void Del(long id)
        {
            this._DA_AccessCompetency.Del(id);
        }
    }
}
