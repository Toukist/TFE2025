using Dior.Library.BO;
using Dior.Library.DAO;

namespace Dior.Service.Services
{
    public interface ITeamService
    {
        List<Team> GetAll();
        Team? GetById(int id);
        void Create(Team team);
        void Update(Team team);
        void Delete(int id);
    }

    public class TeamService : ITeamService
    {
        private readonly ITeamDao _dao;
        public TeamService(ITeamDao dao)
        {
            _dao = dao;
        }
        public List<Team> GetAll() => _dao.GetAll();
        public Team? GetById(int id) => _dao.GetById(id);
        public void Create(Team team) => _dao.Create(team);
        public void Update(Team team) => _dao.Update(team);
        public void Delete(int id) => _dao.Delete(id);
    }
}