using Dior.Library.Entities;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleDefinitionPrivilegeService
    {
        List<Dior.Library.Entities.RoleDefinitionPrivilege> GetList();
        Dior.Library.Entities.RoleDefinitionPrivilege Get(long id);
        long Add(Dior.Library.Entities.RoleDefinitionPrivilege entity, string editBy);
        void Set(Dior.Library.Entities.RoleDefinitionPrivilege entity, string editBy);
        void Del(long id);
    }
}