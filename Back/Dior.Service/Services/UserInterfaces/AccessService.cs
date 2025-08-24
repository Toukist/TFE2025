using Dior.Library.DTO;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Interfaces.DAOs; // Nouvelle interface
using Dior.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Dior.Library.Entities;

namespace Dior.Service.Services.UserInterfaces
{
    public class AccessService : IAccessService
    {
        private readonly string _connectionString;
        private readonly IDA_Access _daAccess;
        private readonly DiorDbContext _context;

        public AccessService(IDA_Access dA_Access, IConfiguration configuration, DiorDbContext context)
        {
            _daAccess = dA_Access ?? throw new ArgumentNullException(nameof(dA_Access));
            _connectionString = configuration?.GetConnectionString("Dior_DB") ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<AccessDto> GetList()
        {
            var accesses = _daAccess.GetAllAccesses();
            return accesses.Select(a => new AccessDto
            {
                Id = a.Id,
                Name = a.Name ?? string.Empty,
                Description = a.Description,
                IsActive = a.IsActive
            }).ToList();
        }
        
        public long Add(Access item, string editBy) => throw new NotImplementedException();
        public void Set(Access item, string editBy) => throw new NotImplementedException();
        public void Del(long id) => throw new NotImplementedException();

        public async Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct)
        {
            var access = await _context.Access
                .FirstOrDefaultAsync(a => a.Id == id, ct)
                .ConfigureAwait(false);
            
            if (access == null)
                return false;

            access.IsActive = isActive;
            await _context.SaveChangesAsync(ct).ConfigureAwait(false);
            return true;
        }

        public Task<IEnumerable<AccessDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AccessDto?> GetByIdAsync(int id)
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

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}