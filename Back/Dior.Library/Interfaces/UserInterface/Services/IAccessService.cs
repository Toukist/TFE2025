using Dior.Library.DTO.Access;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAccessService
    {
        Task<IEnumerable<AccessDto>> GetAllAsync();
        Task<AccessDto?> GetByIdAsync(int id);
        Task<AccessDto> CreateAsync(CreateAccessDto createDto);
        Task<bool> UpdateAsync(int id, UpdateAccessDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}