using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;

namespace Dior.Service.Services.UserInterfaces
{
    public class RoleDefinitionPrivilegeService : IRoleDefinitionPrivilegeService
    {
        private readonly IDA_RoleDefinitionPrivilege _dao;

        public RoleDefinitionPrivilegeService(IDA_RoleDefinitionPrivilege dao)
        {
            _dao = dao;
        }

        public List<RoleDefinitionPrivilege> GetList()
        {
            return _dao.GetList();
        }

        public RoleDefinitionPrivilege Get(int id)
        {
            return _dao.Get(id);
        }

        public int Add(RoleDefinitionPrivilege item, string editBy)
        {
            throw new NotImplementedException();
        }

        public void Set(RoleDefinitionPrivilege item, string editBy)
        {
            throw new NotImplementedException();
        }

        public void Del(int id)
        {
            _dao.Del(id);
        }
    }
}