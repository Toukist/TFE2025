using Dior.Library.Interfaces.UserInterface.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Library.Services.Interfaces
{
    /// <summary>
    /// Service de gestion des UserAccessCompetency
    /// </summary>
    public interface IUserAccessCompetencyService
    {
        Task<IEnumerable<UserAccessCompetencyDto>> GetAllAsync();
        Task<UserAccessCompetencyDto> GetByIdAsync(long id);
        Task<IEnumerable<UserAccessCompetencyDto>> GetByUserIdAsync(long userId);
        Task<UserAccessCompetencyDto> CreateAsync(CreateUserAccessCompetencyDto createDto);
<<<<<<< Updated upstream
        Task<bool> DeleteAsync(int id);
=======
        Task<bool> UpdateAsync(long id, UpdateUserAccessCompetencyDto updateDto);
        Task<bool> DeleteAsync(long id);
>>>>>>> Stashed changes
    }
}