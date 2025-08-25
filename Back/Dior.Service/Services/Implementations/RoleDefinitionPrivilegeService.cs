using AutoMapper;
<<<<<<< Updated upstream
using Dior.Data.DTO;
using Dior.Database.Services.Interfaces;
using Dior.Library.BO.UserInterface;
=======
using Dior.Service.Host.Services;
using Dior.Library.DTO.Role;
using Dior.Library.Entities;
using Dior.Service.Services.Interfaces;
>>>>>>> Stashed changes
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services.Implementations
{
    /// <summary>
    /// Service liaisons rôle-privilège
    /// </summary>
    public class RoleDefinitionPrivilegeService : IRoleDefinitionPrivilegeService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>Constructor</summary>
        public RoleDefinitionPrivilegeService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>Retourne toutes les liaisons</summary>
        public async Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetAllAsync()
        {
            var roleDefPrivileges = await _context.RoleDefinitionPrivileges.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDefinitionPrivilegeDto>>(roleDefPrivileges);
        }

        /// <summary>Retourne une liaison par id</summary>
        public async Task<RoleDefinitionPrivilegeDto?> GetByIdAsync(long id)
        {
            var roleDefPrivilege = await _context.RoleDefinitionPrivileges.FindAsync(id);
            return roleDefPrivilege != null ? _mapper.Map<RoleDefinitionPrivilegeDto>(roleDefPrivilege) : null;
        }

        /// <summary>Retourne les liaisons d'un rôle</summary>
        public async Task<IEnumerable<RoleDefinitionPrivilegeDto>> GetByRoleDefinitionIdAsync(long roleDefinitionId)
        {
            var roleDefPrivileges = await _context.RoleDefinitionPrivileges
                .Where(rdp => rdp.RoleDefinitionId == roleDefinitionId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoleDefinitionPrivilegeDto>>(roleDefPrivileges);
        }

        /// <summary>Crée une liaison</summary>
        public async Task<RoleDefinitionPrivilegeDto> CreateAsync(CreateRoleDefinitionPrivilegeDto createDto)
        {
            var roleDefPrivilege = _mapper.Map<RoleDefinitionPrivilege>(createDto);
            roleDefPrivilege.CreatedAt = DateTime.UtcNow;

            _context.RoleDefinitionPrivileges.Add(roleDefPrivilege);
            await _context.SaveChangesAsync();

            return _mapper.Map<RoleDefinitionPrivilegeDto>(roleDefPrivilege);
        }

        /// <summary>Met à jour une liaison</summary>
        public async Task<bool> UpdateAsync(long id, UpdateRoleDefinitionPrivilegeDto updateDto)
        {
            var roleDefPrivilege = await _context.RoleDefinitionPrivileges.FindAsync(id);
            if (roleDefPrivilege == null)
                return false;

            _mapper.Map(updateDto, roleDefPrivilege);
            roleDefPrivilege.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime une liaison</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var roleDefPrivilege = await _context.RoleDefinitionPrivileges.FindAsync(id);
            if (roleDefPrivilege == null)
                return false;

            _context.RoleDefinitionPrivileges.Remove(roleDefPrivilege);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}