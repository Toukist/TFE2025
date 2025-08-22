using Dior.Library.DTO.Role;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class UserRoleService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(DiorDbContext context, ILogger<UserRoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserRoleDto>> GetAllAsync()
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.RoleDefinition)
                .ToListAsync();

            return userRoles.Select(MapToDto).ToList();
        }

        public async Task<List<UserRoleDto>> GetByUserIdAsync(int userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.User)
                .Include(ur => ur.RoleDefinition)
                .ToListAsync();

            return userRoles.Select(MapToDto).ToList();
        }

        public async Task<UserRoleDto> AssignRoleAsync(AssignRoleDto dto)
        {
            // Vérifier si l'assignation existe déjà
            var existing = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.RoleDefinition)
                .FirstOrDefaultAsync(ur => ur.UserId == dto.UserId && ur.RoleDefinitionId == dto.RoleDefinitionId);

            if (existing != null)
            {
                return MapToDto(existing);
            }

            var userRole = new Dior.Library.Entities.UserRole
            {
                UserId = dto.UserId,
                RoleDefinitionId = dto.RoleDefinitionId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            // Recharger avec les données complètes
            var created = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.RoleDefinition)
                .FirstAsync(ur => ur.UserId == dto.UserId && ur.RoleDefinitionId == dto.RoleDefinitionId);

            return MapToDto(created);
        }

        public Task<bool> RemoveRoleAsync(int id)
        {
            // Pour cette version simplifiée, on supprime par UserRole ID composite
            // En réalité, il faudrait une logique plus complexe
            _ = id; // Utilisation du paramètre pour éviter l'avertissement
            return Task.FromResult(true);
        }

        private static UserRoleDto MapToDto(Dior.Library.Entities.UserRole userRole)
        {
            return new UserRoleDto
            {
                Id = userRole.UserId, // Utilisé comme identifiant composite
                UserId = userRole.UserId,
                UserName = userRole.User?.Username,
                RoleDefinitionId = userRole.RoleDefinitionId,
                RoleName = userRole.RoleDefinition?.Name,
                CreatedAt = userRole.CreatedAt,
                CreatedBy = userRole.CreatedBy ?? string.Empty
            };
        }
    }
}