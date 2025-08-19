namespace Dior.Library.Service.DAO
{
    public interface IDA_RoleDefinitionPrivilege
    {
        long Add(RoleDefinitionPrivilege link, string editBy);
        void Set(RoleDefinitionPrivilege link, string editBy);
        void Del(long id);
        RoleDefinitionPrivilege Get(long id);
        List<RoleDefinitionPrivilege> GetList();
    }
}
