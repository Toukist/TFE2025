using Dior.Library.BO.UserInterface;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleDefinitionService
    {
        RoleDefinition Get(long id);
        List<RoleDefinition> GetList();
        long Add(RoleDefinition roleDefinition, string editBy);
        void Set(RoleDefinition roleDefinition, string editBy);
        void Del(long id);
    }
}
