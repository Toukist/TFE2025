using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
using Dior.Library.Entities;

namespace Dior.Service.Services.UserInterfaces
{
    public class RoleDefinitionService : IRoleDefinitionService
    {
        private readonly IDA_RoleDefinition _daRoleDefinition;

        public IDA_RoleDefinition DaRoleDefinition => _daRoleDefinition;

        public RoleDefinitionService(IDA_RoleDefinition daRoleDefinition)
        {
            _daRoleDefinition = daRoleDefinition;
        }

        public List<RoleDefinition> GetList()
        {
            return DaRoleDefinition.GetList();
        }

        public RoleDefinition Get(long id)
        {
            return DaRoleDefinition.Get((int)id);
        }

        public long Add(RoleDefinition entity, string editBy)
        {
            return DaRoleDefinition.Add(entity, editBy);
        }

        public void Set(RoleDefinition entity, string editBy)
        {
            DaRoleDefinition.Set(entity, editBy);
        }

        public void Del(long id)
        {
            DaRoleDefinition.Del(id);
        }
    }
}
