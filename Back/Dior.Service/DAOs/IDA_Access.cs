using Dior.Library.Entities;

namespace Dior.Library.Interfaces.DAOs 
{
    public interface IDA_Access 
    {
        Access? GetAccessById(int id);
        IEnumerable<Access> GetAllAccesses();
        void CreateAccess(Access access);
        void UpdateAccess(Access access);
        void DeleteAccess(int id);
    }
}