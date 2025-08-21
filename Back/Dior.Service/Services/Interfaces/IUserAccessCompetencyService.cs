using Dior.Database.DTOs.UserAccessCompetency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Services.Interfaces
{
    public interface IUserAccessCompetencyService
    {
        Task<IEnumerable<UserAccessCompetencyDto>> GetAllAsync();
        Task<UserAccessCompetencyDto> GetByIdAsync(int id);
        Task<IEnumerable<UserAccessCompetencyDto>> GetByUserIdAsync(int userId);
        Task<UserAccessCompetencyDto> CreateAsync(CreateUserAccessCompetencyDto createDto);
        Task<bool> UpdateAsync(int id, UpdateUserAccessCompetencyDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}