using Dior.Library.Entities;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IRoleDefinitionService
    {
        List<Dior.Library.Entities.RoleDefinition> GetList();
        Dior.Library.Entities.RoleDefinition Get(long id);
        long Add(Dior.Library.Entities.RoleDefinition entity, string editBy);
        void Set(Dior.Library.Entities.RoleDefinition entity, string editBy);
        void Del(long id);
    }
}
