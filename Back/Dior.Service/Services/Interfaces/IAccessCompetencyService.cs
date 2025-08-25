using Dior.Data.DTO.Access;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Data.Services.Interfaces
{
    public interface IAccessCompetencyService
    {
        Task<IEnumerable<AccessCompetencyDto>> GetAllAsync();
        Task<AccessCompetencyDto> GetByIdAsync(int id);
        Task<AccessCompetencyDto> CreateAsync(CreateAccessCompetencyDto createAccessCompetencyDto);
        Task<bool> UpdateAsync(int id, UpdateAccessCompetencyDto updateAccessCompetencyDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<AccessCompetencyDto>> GetChildrenAsync(int parentId);
    }
}