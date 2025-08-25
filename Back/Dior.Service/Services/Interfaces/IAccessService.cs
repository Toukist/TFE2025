using Dior.Data.DTOs.Access;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Services.Interfaces
{
    public interface IAccessService
    {
        Task<IEnumerable<AccessDto>> GetAllAsync();
        Task<AccessDto> GetByIdAsync(int id);
        Task<AccessDto> CreateAsync(CreateAccessDto createAccessDto);
        Task<bool> UpdateAsync(int id, UpdateAccessDto updateAccessDto);
        Task<bool> DeleteAsync(int id);
    }
}