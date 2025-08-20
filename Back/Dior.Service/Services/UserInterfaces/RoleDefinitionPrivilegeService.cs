using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.Entities;

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

        public long Add(RoleDefinitionPrivilege entity, string editBy)
        {
            return _dao.Add(entity, editBy);
        }

        public void Set(RoleDefinitionPrivilege entity, string editBy)
        {
            _dao.Set(entity, editBy);
        }

        public void Del(int id)
        {
            _dao.Del(id);
        }
    }
}