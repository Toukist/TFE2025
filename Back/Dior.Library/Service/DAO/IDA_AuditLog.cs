namespace Dior.Library.Service.DAO
{
    public interface IDA_AuditLog
    {
        List<Entities.AuditLog> GetList();
        Entities.AuditLog Get(long id);
        long Add(Entities.AuditLog entity, string editBy);
        void Set(Entities.AuditLog entity, string editBy);
        void Del(long id);
    }
}