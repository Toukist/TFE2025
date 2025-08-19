using Dior.Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Services
{
    public class UserAccessCompetencyReader : IUserAccessCompetencyReader
    {
        private readonly DiorDbContext _context;

        public UserAccessCompetencyReader(DiorDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasCompetencyAsync(int userId, string competencyCode)
        {
            return await _context.UserAccessCompetencies
                .AnyAsync(uac => uac.UserId == userId
                    && uac.AccessCompetency.Name == competencyCode);
        }

        public async Task<List<string>> GetUserCompetenciesAsync(int userId)
        {
            return await _context.UserAccessCompetencies
                .Where(uac => uac.UserId == userId)
                .Select(uac => uac.AccessCompetency.Name)
                .ToListAsync();
        }
    }
}