using Dior.Data.DTO.Team;  // pour TeamDto, CreateTeamDto, UpdateTeamDto
using Dior.Library.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;


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