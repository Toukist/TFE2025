using Dior.Library.BO;

namespace Dior.Library.DAO
{
    public interface ITeamDao
    {
        List<Team> GetAll();
        Team? GetById(long id);
        void Create(Team team);
        void Update(Team team);
        void Delete(long id);
    }
}