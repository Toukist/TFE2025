using Dior.Library.DAO;
using Dior.Service.Services; // Pour DiorDbContext
using BO = Dior.Library.BO;
using EF = Dior.Library.Entities;

namespace Dior.Service.DAO
{
    public class DA_Team : ITeamDao
    {
        private readonly DiorDbContext _context;
        public DA_Team(DiorDbContext context)
        {
            _context = context;
        }

        public List<BO.Team> GetAll()
        {
            return _context.Teams.Select(t => new BO.Team
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        public BO.Team? GetById(int id)
        {
            var t = _context.Teams.FirstOrDefault(x => x.Id == id);
            if (t == null) return null;
            return new BO.Team
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            };
        }

        public void Create(BO.Team team)
        {
            var entity = new EF.Team
            {
                Name = team.Name,
                Description = team.Description,
                CreatedAt = team.CreatedAt
            };
            _context.Teams.Add(entity);
            _context.SaveChanges();
            team.Id = entity.Id;
        }

        public void Update(BO.Team team)
        {
            var entity = _context.Teams.FirstOrDefault(x => x.Id == team.Id);
            if (entity == null) return;
            entity.Name = team.Name;
            entity.Description = team.Description;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Teams.FirstOrDefault(x => x.Id == id);
            if (entity == null) return;
            _context.Teams.Remove(entity);
            _context.SaveChanges();
        }
    }
}