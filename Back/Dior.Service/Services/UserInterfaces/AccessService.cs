using Dior.Data.DTO.Access;
using Dior.Library.Entities;
using Dior.Library.Interfaces.DAOs;
using Dior.Library.Interfaces.UserInterface.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dior.Service.Services.UserInterfaces
{
    public class AccessService : IAccessService
    {
        private readonly string _connectionString;
        private readonly IDA_Access _daAccess;
        private readonly DiorDbContext _context;

        public AccessService(IDA_Access daAccess, IConfiguration configuration, DiorDbContext context)
        {
            _daAccess = daAccess ?? throw new ArgumentNullException(nameof(daAccess));
            _connectionString = configuration?.GetConnectionString("Dior_DB")
                                ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AccessDto>> GetAllAsync()
        {
            return await _context.Access
                .Select(a => new AccessDto
                {
                    Id = a.Id,
                    BadgePhysicalNumber = a.BadgePhysicalNumber,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    CreatedBy = a.CreatedBy,
                    LastEditAt = a.LastEditAt,
                    LastEditBy = a.LastEditBy
                })
                .ToListAsync();
        }

        public async Task<AccessDto?> GetByIdAsync(int id)
        {
            return await _context.Access
                .Where(a => a.Id == id)
                .Select(a => new AccessDto
                {
                    Id = a.Id,
                    BadgePhysicalNumber = a.BadgePhysicalNumber,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    CreatedBy = a.CreatedBy,
                    LastEditAt = a.LastEditAt,
                    LastEditBy = a.LastEditBy
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AccessDto> CreateAsync(CreateAccessDto createDto)
        {
            var entity = new Access
            {
                BadgePhysicalNumber = createDto.BadgePhysicalNumber,
                IsActive = createDto.IsActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "System"
            };

            _context.Access.Add(entity);
            await _context.SaveChangesAsync();

            return new AccessDto
            {
                Id = entity.Id,
                BadgePhysicalNumber = entity.BadgePhysicalNumber,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateAccessDto updateDto)
        {
            var access = await _context.Access.FindAsync(id);
            if (access == null) return false;

            if (!string.IsNullOrEmpty(updateDto.BadgePhysicalNumber))
                access.BadgePhysicalNumber = updateDto.BadgePhysicalNumber;

            if (updateDto.IsActive.HasValue)
                access.IsActive = updateDto.IsActive.Value;

            access.LastEditAt = DateTime.Now;
            access.LastEditBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var access = await _context.Access.FindAsync(id);
            if (access == null) return false;

            _context.Access.Remove(access);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Access.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var access = await _context.Access.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (access == null) return false;

            access.IsActive = isActive;
            access.LastEditAt = DateTime.Now;
            access.LastEditBy = "System";

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DisableUserBadgeAsync(int userId, CancellationToken ct = default)
        {
            var access = await _context.Access.FirstOrDefaultAsync(a => a.Id == userId, ct);
            if (access == null) return false;

            access.IsActive = false;
            access.LastEditAt = DateTime.Now;
            access.LastEditBy = "System";

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
