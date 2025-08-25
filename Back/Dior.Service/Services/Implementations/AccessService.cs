using Dior.Data.DTO.Access;
using Dior.Library.DTO.Access;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Service.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dior.Service.Services.Implementations
{
    /// <summary>
    /// Service pour la gestion des accès
    /// </summary>
    public class AccessService : IAccessService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<AccessService> _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        public AccessService(DiorDbContext context, ILogger<AccessService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>Retourne tous les accès</summary>
        public async Task<IEnumerable<AccessDto>> GetAllAsync()
        {
            var accesses = await _context.Accesses.ToListAsync();
            return accesses.Select(MapToDto);
        }

        /// <summary>Retourne un accès par id</summary>
        public async Task<AccessDto?> GetByIdAsync(long id)
        {
            var access = await _context.Accesses.FindAsync(id);
            return access != null ? MapToDto(access) : null;
        }

        /// <summary>Crée un accès</summary>
        public async Task<AccessDto> CreateAsync(CreateAccessDto createDto)
        {
            var access = new Dior.Library.Entities.Access
            {
                BadgePhysicalNumber = createDto.BadgePhysicalNumber,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Accesses.Add(access);
            await _context.SaveChangesAsync();
            
            return MapToDto(access);
        }

        /// <summary>Met à jour un accès</summary>
        public async Task<bool> UpdateAsync(long id, UpdateAccessDto updateDto)
        {
            var access = await _context.Accesses.FindAsync(id);
            if (access == null)
                return false;

            if (!string.IsNullOrEmpty(updateDto.BadgePhysicalNumber))
                access.BadgePhysicalNumber = updateDto.BadgePhysicalNumber;
            if (updateDto.IsActive.HasValue)
                access.IsActive = updateDto.IsActive.Value;

            access.LastEditAt = DateTime.UtcNow;
            access.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Supprime un accès</summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var access = await _context.Accesses.FindAsync(id);
            if (access == null)
                return false;

            _context.Accesses.Remove(access);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>Vérifie l'existence d'un accès</summary>
        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Accesses.AnyAsync(a => a.Id == id);
        }

        private static AccessDto MapToDto(Dior.Library.Entities.Access access)
        {
            return new AccessDto
            {
                Id = access.Id,
                BadgePhysicalNumber = access.BadgePhysicalNumber,
                IsActive = access.IsActive,
                CreatedAt = access.CreatedAt,
                CreatedBy = access.CreatedBy,
                LastEditAt = access.LastEditAt,
                LastEditBy = access.LastEditBy
            };
        }

        Task<IEnumerable<AccessDto>> IAccessService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<AccessDto?> IAccessService.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AccessDto> CreateAsync(CreateAccessDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, UpdateAccessDto updateDto)
        {
            throw new NotImplementedException();
        }
    }
}