namespace Dior.Library.Service.DAO
{
    public interface IDA_RoleDefinition
    {
        long Add(RoleDefinition roleDefinition, string editBy);
        void Set(RoleDefinition roleDefinition, string editBy);
        void Del(long id);
        RoleDefinition Get(RoleDefinition roleDefinition);
        List<RoleDefinition> GetList();
        RoleDefinition Get(int id);
    }
}