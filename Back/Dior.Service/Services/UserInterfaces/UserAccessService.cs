using Dior.Library.Interfaces.UserInterface.Services;

namespace Dior.Service.Services.UserInterfaces
{
    public class UserAccessService : IUserAccessService
    {
        private readonly DiorDbContext _context;

        public UserAccessService(DiorDbContext context)
        {
            _context = context;
        }

        public List<UserAccess> GetList()
        {
            return _context.UserAccesses.ToList();
        }

        public UserAccess Get(long id)
        {
            return _context.UserAccesses.Find(id);
        }

        public long Add(UserAccess userAccess, string editBy)
        {
            userAccess.CreatedAt = DateTime.UtcNow;
            userAccess.CreatedBy = editBy;
            _context.UserAccesses.Add(userAccess);
            _context.SaveChanges();
            return userAccess.Id;
        }

        public void Set(UserAccess userAccess, string editBy)
        {
            var entity = _context.UserAccesses.Find(userAccess.Id);
            if (entity == null) return;
            // Mapper les propriétés si besoin
            entity.LastEditAt = DateTime.UtcNow;
            entity.LastEditBy = editBy;
            _context.SaveChanges();
        }

        public void Del(long id)
        {
            var entity = _context.UserAccesses.Find(id);
            if (entity == null) return;
            _context.UserAccesses.Remove(entity);
            _context.SaveChanges();
        }

        public int? GetActiveAccessIdByUserId(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
