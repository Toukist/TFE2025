using Dior.Library.DTO.Role;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class RoleService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(DiorDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<RoleDefinitionDto>> GetAllRolesAsync()
        {
            var roles = await _context.RoleDefinitions
                .Include(r => r.RoleDefinitionPrivileges)
                .ThenInclude(rp => rp.Privilege)
                .ToListAsync();

            return roles.Select(MapToDto).ToList();
        }

        public async Task<RoleDefinitionDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.RoleDefinitions
                .Include(r => r.RoleDefinitionPrivileges)
                .ThenInclude(rp => rp.Privilege)
                .FirstOrDefaultAsync(r => r.Id == id);

            return role != null ? MapToDto(role) : null;
        }

        public Task<RoleDefinitionDto> CreateRoleAsync(CreateRoleDto dto)
        {
            // Version mockée temporaire
            var role = new RoleDefinitionDto
            {
                Id = new Random().Next(1000, 9999),
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            return Task.FromResult(role);
        }

        public Task<bool> UpdateRoleAsync(int id, UpdateRoleDto dto)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteRoleAsync(int id)
        {
            return Task.FromResult(true);
        }

        private static RoleDefinitionDto MapToDto(Dior.Library.Entities.RoleDefinition role)
        {
            return new RoleDefinitionDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                ParentRoleId = role.ParentRoleId,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy ?? string.Empty,
                LastEditAt = role.LastEditAt,
                LastEditBy = role.LastEditBy,
                Privileges = role.RoleDefinitionPrivileges?.Select(rp => new PrivilegeDto
                {
                    Id = rp.Privilege.Id,
                    Name = rp.Privilege.Name,
                    Description = rp.Privilege.Description,
                    IsActive = rp.Privilege.IsActive,
                    CreatedAt = rp.Privilege.CreatedAt,
                    CreatedBy = rp.Privilege.CreatedBy ?? string.Empty
                }).ToList()
            };
        }
    }
}