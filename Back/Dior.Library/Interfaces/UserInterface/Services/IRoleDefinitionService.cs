using Dior.Library.BO.UserInterface;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleDefinitionService
    {
        List<RoleDefinition> GetList();
        RoleDefinition Get(long id);
        long Add(RoleDefinition entity, string editBy);
        void Set(RoleDefinition entity, string editBy);
        void Del(long id);
    }
}
