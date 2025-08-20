using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.Entities;

namespace Dior.Service.Services.UserInterfaces
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IDA_Privilege _daPrivilege;

        public PrivilegeService(IDA_Privilege daPrivilege)
        {
            _daPrivilege = daPrivilege;
        }

        public Privilege Get(long id)
        {
            return _daPrivilege.Get((int)id);
        }

        public List<Privilege> GetList()
        {
            return _daPrivilege.GetList();
        }

        public long Add(Privilege privilege, string editBy)
        {
            return _daPrivilege.Add(privilege, editBy);
        }

        public void Set(Privilege privilege, string editBy)
        {
            _daPrivilege.Set(privilege, editBy);
        }

        public void Del(long id)
        {
            _daPrivilege.Del((int)id);
        }
    }
}
