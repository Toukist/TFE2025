using Dior.Library.BO;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services.Implementations
{
    /// <summary>
    /// Service pour la gestion des équipes
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<TeamService> _logger;

        /// <summary>Constructor</summary>
        public TeamService(DiorDbContext context, ILogger<TeamService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Retourne toutes les équipes</summary>
        public async Task<IEnumerable<TeamDto>> GetAllAsync()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams.Select(MapToDto);
        }

        /// <summary>Retourne une équipe par identifiant</summary>
        public async Task<TeamDto?> GetByIdAsync(long id)
        {
            var team = await _context.Teams.FindAsync(id);
            return team != null ? MapToDto(team) : null;
        }

        /// <summary>Crée une équipe</summary>
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

        /// <summary>Met à jour une équipe</summary>
        public async Task<bool> UpdateAsync(long id, UpdateTeamDto updateDto)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

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

        /// <summary>Supprime une équipe</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Vérifie l'existence d'une équipe</summary>
        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Teams.AnyAsync(t => t.Id == id);
        }

        /// <summary>Retourne les membres d'une équipe</summary>
        public async Task<List<UserDto>> GetTeamMembersAsync(long teamId)
        {
            var users = await _context.Users
                .Where(u => u.TeamId == teamId)
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .ToListAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Phone = u.Phone,
                TeamName = u.Team?.Name
            }).ToList();
        }


        private static TeamDto MapToDto(Dior.Library.Entities.Team team)
        {
            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsActive = team.IsActive,
                CreatedAt = team.CreatedAt,
                CreatedBy = team.CreatedBy,
                LastEditAt = team.LastEditAt,
                LastEditBy = team.LastEditBy,
                MemberCount = 0
            };
        }

        // Legacy signatures not used; keep stubs but align types to long and return defaults
        /// <summary>Non utilisé</summary>
        public List<Team> GetAll()
        {
            return new List<Team>();
        }
        /// <summary>Non utilisé</summary>
        public Team? GetById(long id)
        {
            return null;
        }
        /// <summary>Non utilisé</summary>
        public void Create(Team team)
        {
        }
        /// <summary>Non utilisé</summary>
        public void Update(Team team)
        {
        }
        /// <summary>Non utilisé</summary>
        public void Delete(long id)
        {
        }
    }
}