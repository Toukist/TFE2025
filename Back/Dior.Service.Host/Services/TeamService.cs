using Dior.Library.DTO.Team;
using Dior.Library.DTO.User;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class TeamService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<TeamService> _logger;

        public TeamService(DiorDbContext context, ILogger<TeamService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TeamDto>> GetAllAsync()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams.Select(MapToDto).ToList();
        }

        public async Task<TeamDto?> GetByIdAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            return team != null ? MapToDto(team) : null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Teams.AnyAsync(t => t.Id == id);
        }

        public async Task<TeamDto> CreateAsync(CreateTeamDto createDto)
        {
            var team = new Dior.Library.Entities.Team
            {
                Name = createDto.Name,
                Description = createDto.Description,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return MapToDto(team);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTeamDto updateDto)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return false;

            if (!string.IsNullOrEmpty(updateDto.Name))
                team.Name = updateDto.Name;
            if (updateDto.Description != null)
                team.Description = updateDto.Description;
            if (updateDto.IsActive.HasValue)
                team.IsActive = updateDto.IsActive.Value;

            team.LastEditAt = DateTime.UtcNow;
            team.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetTeamMembersAsync(int teamId)
        {
            var users = await _context.Users
                .Where(u => u.TeamId == teamId)
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .ToListAsync();

            return users.Select(u => new UserDto
            {
                Id = u.ID, // Utiliser ID au lieu de Id
                Username = u.Username, // Utiliser Username au lieu de UserName
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Phone = u.Phone,
                IsActive = u.IsActive,
                TeamId = u.TeamId,
                TeamName = u.Team?.Name,
                BadgePhysicalNumber = u.BadgePhysicalNumber,
                CreatedAt = u.CreatedAt,
                CreatedBy = u.CreatedBy,
                LastEditAt = u.LastEditAt,
                LastEditBy = u.LastEditBy,
                AccessCompetencies = u.UserRoles.Select(ur => ur.RoleDefinition.Name).ToList()
            }).ToList();
        }

        private static TeamDto MapToDto(Dior.Library.Entities.Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description ?? string.Empty,
                IsActive = team.IsActive,
                CreatedAt = team.CreatedAt,
                CreatedBy = team.CreatedBy ?? string.Empty,
                LastEditAt = team.LastEditAt,
                LastEditBy = team.LastEditBy,
                MemberCount = 0 // Sera calculé séparément si nécessaire
            };
        }
    }
}