using Dior.Library.DTO.User;
using Dior.Library.DTO.Access;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class UserService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(DiorDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .ToListAsync();

            return users.Select(MapToDto).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createDto)
        {
            var user = new Dior.Library.Entities.User
            {
                UserName = createDto.UserName,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Phone = createDto.Phone,
                IsActive = createDto.IsActive,
                TeamId = createDto.TeamId,
                BadgePhysicalNumber = createDto.BadgePhysicalNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto updateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(updateDto.UserName))
                user.UserName = updateDto.UserName;
            if (!string.IsNullOrEmpty(updateDto.FirstName))
                user.FirstName = updateDto.FirstName;
            if (!string.IsNullOrEmpty(updateDto.LastName))
                user.LastName = updateDto.LastName;
            if (!string.IsNullOrEmpty(updateDto.Email))
                user.Email = updateDto.Email;
            if (updateDto.Phone != null)
                user.Phone = updateDto.Phone;
            if (updateDto.IsActive.HasValue)
                user.IsActive = updateDto.IsActive.Value;
            if (updateDto.TeamId.HasValue)
                user.TeamId = updateDto.TeamId.Value;

            user.LastEditAt = DateTime.UtcNow;
            user.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserFullDto>> GetFullUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .ToListAsync();

            return users.Select(MapToFullDto).ToList();
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.RoleDefinition)
                .Select(ur => ur.RoleDefinition.Name)
                .ToListAsync();

            return roles;
        }

        public async Task<List<AccessCompetencyDto>> GetUserAccessCompetenciesAsync(int userId)
        {
            var competencies = await _context.UserAccessCompetencies
                .Where(uac => uac.UserId == userId)
                .Include(uac => uac.AccessCompetency)
                .Select(uac => new AccessCompetencyDto
                {
                    Id = uac.AccessCompetency.Id,
                    Name = uac.AccessCompetency.Name,
                    Description = uac.AccessCompetency.Description,
                    ParentId = null, // Propriété non disponible dans l'entité
                    IsActive = uac.AccessCompetency.IsActive,
                    CreatedAt = uac.AccessCompetency.CreatedAt,
                    CreatedBy = uac.AccessCompetency.CreatedBy ?? string.Empty,
                    LastEditAt = uac.AccessCompetency.LastEditAt,
                    LastEditBy = uac.AccessCompetency.LastEditBy
                })
                .ToListAsync();

            return competencies;
        }

        private static UserDto MapToDto(Dior.Library.Entities.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive,
                TeamId = user.TeamId,
                TeamName = user.Team?.Name,
                BadgePhysicalNumber = user.BadgePhysicalNumber,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                LastEditAt = user.LastEditAt,
                LastEditBy = user.LastEditBy,
                Roles = user.UserRoles?.Select(ur => ur.RoleDefinition.Name).ToList() ?? new List<string>()
            };
        }

        private static UserFullDto MapToFullDto(Dior.Library.Entities.User user)
        {
            return new UserFullDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive,
                TeamId = user.TeamId,
                TeamName = user.Team?.Name,
                BadgePhysicalNumber = user.BadgePhysicalNumber,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                LastEditAt = user.LastEditAt,
                LastEditBy = user.LastEditBy,
                Roles = user.UserRoles?.Select(ur => ur.RoleDefinition.Name).ToList() ?? new List<string>(),
                Team = user.Team != null ? new Dior.Library.DTO.Team.TeamDto
                {
                    Id = user.Team.Id,
                    Name = user.Team.Name,
                    Description = user.Team.Description ?? string.Empty,
                    IsActive = user.Team.IsActive,
                    CreatedAt = user.Team.CreatedAt,
                    CreatedBy = user.Team.CreatedBy ?? string.Empty,
                    LastEditAt = user.Team.LastEditAt,
                    LastEditBy = user.Team.LastEditBy,
                    MemberCount = 0
                } : null
            };
        }
    }
}