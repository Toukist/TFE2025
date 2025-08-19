// Dior.Library\Service\DAO\IDA_Access.cs
using Dior.Library.DTO;

namespace Dior.Library.Service.DAO
{
    public interface IDA_Access
    {
        List<AccessDto> GetList();
        Access Get(long id);
        long Add(Access entity, string editBy);
        void Set(Access entity, string editBy);
        void Del(long id);
    }
}