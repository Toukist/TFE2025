namespace Dior.Library.Interfaces.UserInterface.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllAsync();
        Task<TeamDto?> GetByIdAsync(int id);
        Task<TeamDto> CreateAsync(CreateTeamDto createDto);
        Task<bool> UpdateAsync(int id, UpdateTeamDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<UserDto>> GetTeamMembersAsync(int teamId);
    }
}