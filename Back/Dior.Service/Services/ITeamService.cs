using Dior.Library.BO;
using System.Collections.Generic;

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
}