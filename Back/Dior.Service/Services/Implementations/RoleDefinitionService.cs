using AutoMapper;
using Dior.Data.Services.Interfaces;
using Dior.Library.BO.UserInterface;
using Microsoft.EntityFrameworkCore;

namespace Dior.Data.Services.Implementations
{
    /// <summary>
    /// Service de gestion des définitions de rôle
    /// </summary>
    public class RoleDefinitionService : IRoleDefinitionService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>Constructor</summary>
        public RoleDefinitionService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>Retourne toutes les définitions de rôle</summary>
        public async Task<IEnumerable<RoleDefinitionDto>> GetAllAsync()
        {
            var roles = await _context.RoleDefinitions.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDefinitionDto>>(roles);
        }

        /// <summary>Retourne une définition de rôle par id</summary>
        public async Task<RoleDefinitionDto> GetByIdAsync(long id)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            return role != null ? _mapper.Map<RoleDefinitionDto>(role) : null;
        }

        /// <summary>Retourne les rôles enfants d'un parent</summary>
        public async Task<IEnumerable<RoleDefinitionDto>> GetChildRolesAsync(long parentId)
        {
            var childRoles = await _context.RoleDefinitions
                .Where(r => r.ParentRoleId == parentId)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<RoleDefinitionDto>>(childRoles);
        }

        /// <summary>Crée une définition de rôle</summary>
        public async Task<RoleDefinitionDto> CreateAsync(CreateRoleDefinitionDto createRoleDefinitionDto)
        {
            // Correction : Utiliser le bon type d'entité pour l'ajout dans le DbSet
            var roleEntity = _mapper.Map<Dior.Library.Entities.RoleDefinition>(createRoleDefinitionDto);
            roleEntity.CreatedAt = DateTime.UtcNow;

            _context.RoleDefinitions.Add(roleEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDefinitionDto>(roleEntity);
        }

        /// <summary>Met à jour une définition de rôle</summary>
        public async Task<bool> UpdateAsync(long id, UpdateRoleDefinitionDto updateRoleDefinitionDto)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            if (role == null)
                return false;

            _mapper.Map(updateRoleDefinitionDto, role);
            role.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime une définition de rôle</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var role = await _context.RoleDefinitions.FindAsync(id);
            if (role == null)
                return false;

            _context.RoleDefinitions.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}