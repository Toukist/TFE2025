using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;

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

        public RoleDefinition Get(long id)
        {
            return DaRoleDefinition.Get((int)id);
        }

        public List<RoleDefinition> GetList()
        {
            return DaRoleDefinition.GetList();
        }

        public long Add(RoleDefinition roleDefinition, string editBy)
        {
            return DaRoleDefinition.Add(roleDefinition, editBy);
        }

        public void Set(RoleDefinition roleDefinition, string editBy)
        {
            DaRoleDefinition.Set(roleDefinition, editBy);
        }

        public void Del(long id)
        {
            DaRoleDefinition.Del(id);
        }
    }
}
