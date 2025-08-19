using Dior.Library.DTO;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAccessService
    {
        List<AccessDto> GetList();
        long Add(Access item, string editBy);
        void Set(Access item, string editBy);
        void Del(long id);
        Task<bool> SetActiveAsync(int id, bool v, CancellationToken ct);
    }
}