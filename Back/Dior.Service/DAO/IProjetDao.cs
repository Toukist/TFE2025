using Dior.Library.BO;

namespace Dior.Library.DAO
{
    public interface IProjetDao
    {
        List<Projet> GetAll();
        Projet? GetById(int id);
        List<Projet> GetByTeamId(int teamId);
        int Create(Projet projet);
        bool Update(Projet projet);
        bool Delete(int id);
    }
}