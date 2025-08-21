using Microsoft.EntityFrameworkCore;
using Dior.Library.Interfaces.UserInterface.Services;

namespace Dior.Database.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(DiorDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .ToListAsync();
            
            return users.Select(MapToDto);
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

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .FirstOrDefaultAsync(u => u.Email == email);
            
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Team)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.RoleDefinition)
                .FirstOrDefaultAsync(u => u.UserName == username);
            
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
        {
            var user = new Dior.Library.Entities.User
            {
                UserName = createUserDto.UserName,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Phone = createUserDto.Phone,
                IsActive = createUserDto.IsActive,
                TeamId = createUserDto.TeamId,
                BadgePhysicalNumber = createUserDto.BadgePhysicalNumber,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            
            // Hash the password
            if (!string.IsNullOrEmpty(createUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return MapToDto(user);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            // Mise à jour des propriétés
            if (!string.IsNullOrEmpty(updateUserDto.UserName))
                user.UserName = updateUserDto.UserName;
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;
            if (!string.IsNullOrEmpty(updateUserDto.Email))
                user.Email = updateUserDto.Email;
            if (updateUserDto.Phone != null)
                user.Phone = updateUserDto.Phone;
            if (updateUserDto.IsActive.HasValue)
                user.IsActive = updateUserDto.IsActive.Value;
            if (updateUserDto.TeamId.HasValue)
                user.TeamId = updateUserDto.TeamId.Value;
            if (!string.IsNullOrEmpty(updateUserDto.BadgePhysicalNumber))
                user.BadgePhysicalNumber = updateUserDto.BadgePhysicalNumber;

            user.LastEditAt = DateTime.UtcNow;
            user.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
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
                    IsActive = uac.AccessCompetency.IsActive,
                    CreatedAt = uac.AccessCompetency.CreatedAt,
                    CreatedBy = uac.AccessCompetency.CreatedBy
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
                    Description = user.Team.Description,
                    IsActive = user.Team.IsActive,
                    CreatedAt = user.Team.CreatedAt,
                    CreatedBy = user.Team.CreatedBy
                } : null
            };
        }
    }
}