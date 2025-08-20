using Dior.Library.DTO;
using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Library.Service.DAO;
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
            this._daAccess = dA_Access;
            this._connectionString = configuration.GetConnectionString("Dior_DB");
            this._context = context;
        }

        public List<AccessDto> GetList()
        {
            return _daAccess.GetList();
        }
        public long Add(Access item, string editBy) => throw new NotImplementedException();
        public void Set(Access item, string editBy) => throw new NotImplementedException();
        public void Del(long id) => throw new NotImplementedException();

        public async Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct)
        {
            var access = await _context.Access.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (access == null)
                return false;

            access.IsActive = isActive;
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}