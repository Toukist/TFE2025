using Dior.Library.BO.UserInterface;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleDefinitionPrivilegeService
    {
        List<RoleDefinitionPrivilege> GetList();
        RoleDefinitionPrivilege Get(int id);
        int Add(RoleDefinitionPrivilege item, string editBy);
        void Set(RoleDefinitionPrivilege item, string editBy);
        void Del(int id);
    }
}