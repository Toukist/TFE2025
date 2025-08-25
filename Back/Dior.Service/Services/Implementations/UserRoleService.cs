using AutoMapper;
<<<<<<< Updated upstream
using Dior.Database.Services.Interfaces;
using Dior.Library.Entities;
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
    /// Service de gestion des liens utilisateur-rôle
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly DiorDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>Constructor</summary>
        public UserRoleService(DiorDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>Retourne tous les liens</summary>
        public async Task<IEnumerable<UserRoleDto>> GetAllAsync()
        {
            var userRoles = await _context.UserRoles.ToListAsync();
            return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
        }

        /// <summary>Retourne un lien par id</summary>
        public async Task<UserRoleDto?> GetByIdAsync(long id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            return userRole != null ? _mapper.Map<UserRoleDto>(userRole) : null;
        }

        /// <summary>Retourne les liens pour un utilisateur</summary>
        public async Task<IEnumerable<UserRoleDto>> GetByUserIdAsync(long userId)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserRoleDto>>(userRoles);
        }

        /// <summary>Crée un lien</summary>
        public async Task<UserRoleDto> CreateAsync(CreateUserRoleDto createDto)
        {
            var userRole = _mapper.Map<UserRole>(createDto);
            userRole.CreatedAt = DateTime.UtcNow;

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserRoleDto>(userRole);
        }

        /// <summary>Met à jour un lien</summary>
        public async Task<bool> UpdateAsync(long id, UpdateUserRoleDto updateDto)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
                return false;

            _mapper.Map(updateDto, userRole);
            userRole.LastEditAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime un lien</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
                return false;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}