using Dior.Data.DTO.Access;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface IAccessService
    {
        Task<IEnumerable<AccessDto>> GetAllAsync();
        Task<AccessDto?> GetByIdAsync(long id);
        Task<AccessDto> CreateAsync(CreateAccessDto createDto);
        Task<bool> UpdateAsync(long id, UpdateAccessDto updateDto);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
    }
}