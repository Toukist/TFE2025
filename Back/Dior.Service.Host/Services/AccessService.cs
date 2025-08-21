using Dior.Library.DTO.Access;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class AccessService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<AccessService> _logger;

        public AccessService(DiorDbContext context, ILogger<AccessService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AccessDto>> GetAllAsync()
        {
            var accesses = await _context.Accesses.ToListAsync();
            return accesses.Select(MapToDto).ToList();
        }

        public async Task<AccessDto?> GetByIdAsync(int id)
        {
            var access = await _context.Accesses.FindAsync(id);
            return access != null ? MapToDto(access) : null;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var access = await _context.Accesses.FindAsync(id);
            if (access == null) return false;

            access.IsActive = isActive;
            access.LastEditAt = DateTime.UtcNow;
            access.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisableUserBadgeAsync(int userId)
        {
            var userAccesses = await _context.UserAccesses
                .Where(ua => ua.UserId == userId)
                .Include(ua => ua.Access)
                .ToListAsync();

            foreach (var userAccess in userAccesses)
            {
                userAccess.Access.IsActive = false;
                userAccess.Access.LastEditAt = DateTime.UtcNow;
                userAccess.Access.LastEditBy = "Self-Disabled";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static AccessDto MapToDto(Dior.Library.Entities.Access access)
        {
            return new AccessDto
            {
                Id = access.Id,
                BadgePhysicalNumber = access.BadgePhysicalNumber,
                IsActive = access.IsActive,
                CreatedAt = access.CreatedAt,
                CreatedBy = access.CreatedBy ?? string.Empty,
                LastEditAt = access.LastEditAt,
                LastEditBy = access.LastEditBy
            };
        }
    }
}