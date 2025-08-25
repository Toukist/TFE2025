
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    public interface IPrivilegeService
    {
        Task<IEnumerable<PrivilegeDto>> GetAllAsync();
        Task<PrivilegeDto> GetByIdAsync(int id);
        Task<PrivilegeDto> CreateAsync(CreatePrivilegeDto createPrivilegeDto);
        Task<bool> UpdateAsync(int id, UpdatePrivilegeDto updatePrivilegeDto);
        Task<bool> DeleteAsync(int id);
    }
}