namespace Dior.Library.Service.DAO
{
    public interface IDA_UserRole
    {
        long Add(UserRole userRole, string editBy);
        void Set(UserRole userRole, string editBy);
        void Del(long id);
        UserRole Get(long id);
        List<UserRole> GetList();
    }
}