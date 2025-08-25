using Dior.Library.DTO.Role;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Service.Host.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Service.Services.Implementations
{
    /// <summary>
    /// Service pour la gestion des rôles
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<RoleService> _logger;

        /// <summary>Constructor</summary>
        public RoleService(DiorDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Retourne tous les rôles</summary>
        public async Task<IEnumerable<RoleDefinitionDto>> GetAllAsync()
        {
            var roles = await _context.RoleDefinitions
                .Include(r => r.RoleDefinitionPrivileges)
                .ThenInclude(rp => rp.Privilege)
                .ToListAsync();
            
            return roles.Select(MapToDto);
        }

        /// <summary>Retourne un rôle par id</summary>
        public async Task<RoleDefinitionDto?> GetByIdAsync(long id)
        {
            var role = await _context.RoleDefinitions
                .Include(r => r.RoleDefinitionPrivileges)
                .ThenInclude(rp => rp.Privilege)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            return role != null ? MapToDto(role) : null;
        }

        /// <summary>Crée un rôle</summary>
        public async Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createDto)
        {
            var role = new Dior.Library.Entities.RoleDefinition
            {
                Name = createDto.Name,
                Description = createDto.Description,
                ParentRoleId = createDto.ParentRoleId,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.RoleDefinitions.Add(role);
            await _context.SaveChangesAsync();
            
            return MapToDto(role);
        }

        /// <summary>Met à jour un rôle</summary>
        public async Task<bool> UpdateAsync(long id, UpdateRoleDefinitionDto updateDto)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            if (role == null)
                return false;

            if (!string.IsNullOrEmpty(updateDto.Name))
                role.Name = updateDto.Name;
            if (updateDto.Description != null)
                role.Description = updateDto.Description;
            if (updateDto.ParentRoleId.HasValue)
                role.ParentRoleId = updateDto.ParentRoleId;
            if (updateDto.IsActive.HasValue)
                role.IsActive = updateDto.IsActive.Value;

            role.LastEditAt = DateTime.UtcNow;
            role.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime un rôle</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            if (role == null)
                return false;

            _context.RoleDefinitions.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Vérifie l'existence d'un rôle</summary>
        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.RoleDefinitions.AnyAsync(r => r.Id == id);
        }

        /// <summary>Retourne les noms des rôles actifs</summary>
        public async Task<List<string>> GetRoleNamesAsync()
        {
            return await _context.RoleDefinitions
                .Where(r => r.IsActive)
                .Select(r => r.Name)
                .ToListAsync();
        }

        /// <summary>Retourne les privilèges d'un rôle</summary>
        public async Task<List<PrivilegeDto>> GetRolePrivilegesAsync(long roleId)
        {
            var privileges = await _context.RoleDefinitionPrivileges
                .Where(rp => rp.RoleDefinitionId == roleId)
                .Include(rp => rp.Privilege)
                .Select(rp => new PrivilegeDto
                {
                    Id = rp.Privilege.Id,
                    Name = rp.Privilege.Name,
                    Description = rp.Privilege.Description,
                    IsActive = rp.Privilege.IsActive,
                    CreatedAt = rp.Privilege.CreatedAt,
                    CreatedBy = rp.Privilege.CreatedBy
                })
                .ToListAsync();
            
            return privileges;
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
                CreatedBy = role.CreatedBy,
                LastEditAt = role.LastEditAt,
                LastEditBy = role.LastEditBy,
                Privileges = role.RoleDefinitionPrivileges?.Select(rp => new PrivilegeDto
                {
                    Id = rp.Privilege.Id,
                    Name = rp.Privilege.Name,
                    Description = rp.Privilege.Description,
                    IsActive = rp.Privilege.IsActive,
                    CreatedAt = rp.Privilege.CreatedAt,
                    CreatedBy = rp.Privilege.CreatedBy
                }).ToList()
            };
        }

        public List<RoleDefinitionDto> GetRolesByUserId(long userId)
        {
            throw new NotImplementedException();
        }
    }
}