using Dior.Library.BO.UserInterface;

namespace Dior.Library.Service.DAO
{
    public interface IDA_RoleDefinition
    {
        int Add(RoleDefinition roleDefinition, string editBy);
        void Set(RoleDefinition roleDefinition, string editBy);
        void Del(int id);
        RoleDefinition Get(RoleDefinition roleDefinition);
        List<RoleDefinition> GetList();
        RoleDefinition Get(int id);
    }
}