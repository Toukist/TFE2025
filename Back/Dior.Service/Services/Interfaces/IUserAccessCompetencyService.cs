using Dior.Library.Interfaces.UserInterface.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Services.Interfaces
{
    public interface IUserAccessCompetencyService
    {
        Task<IEnumerable<UserAccessCompetencyDto>> GetAllAsync();
        Task<UserAccessCompetencyDto> GetByIdAsync(int id);
        Task<IEnumerable<UserAccessCompetencyDto>> GetByUserIdAsync(int userId);
        Task<UserAccessCompetencyDto> CreateAsync(CreateUserAccessCompetencyDto createDto);
        Task<bool> DeleteAsync(int id);
    }
}