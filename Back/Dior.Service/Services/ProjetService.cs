using Dior.Library.BO;
using Dior.Library.DAO;
using Dior.Library.DTO;
using Dior.Service.Services;

namespace Dior.Service.Services
{
    public class ProjetService : IProjetService
    {
        private readonly IProjetDao _projetDao;
        private readonly ITeamService _teamService;

        public ProjetService(IProjetDao projetDao, ITeamService teamService)
        {
            _projetDao = projetDao;
            _teamService = teamService;
        }

        public async Task<List<ProjetDto>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                var projets = _projetDao.GetAll();
                return projets.Select(MapToDto).ToList();
            });
        }

        public async Task<ProjetDto?> GetByIdAsync(int id)
        {
            return await Task.Run(() =>
            {
                var projet = _projetDao.GetById(id);
                return projet == null ? null : MapToDto(projet);
            });
        }

        public async Task<List<ProjetDto>> GetByTeamIdAsync(int teamId)
        {
            return await Task.Run(() =>
            {
                var projets = _projetDao.GetByTeamId(teamId);
                return projets.Select(MapToDto).ToList();
            });
        }

        public async Task<List<ProjetDto>> GetByManagerIdAsync(int managerId)
        {
            return await Task.Run(() =>
            {
                var projets = _projetDao.GetAll().Where(p => false).ToList();
                return projets.Select(MapToDto).ToList();
            });
        }

        public async Task<ProjetDto> CreateAsync(CreateProjetRequest request, string createdBy)
        {
            return await Task.Run(() =>
            {
                var projet = new Projet
                {
                    Nom = request.Nom,
                    Description = request.Description,
                    DateDebut = request.DateDebut,
                    DateFin = request.DateFin,
                    TeamId = request.TeamId ?? 1,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                var newId = _projetDao.Create(projet);
                projet.Id = newId;

                return MapToDto(projet);
            });
        }

        public async Task<bool> UpdateAsync(int id, UpdateProjetRequest request, string lastEditBy)
        {
            return await Task.Run(() =>
            {
                var existing = _projetDao.GetById(id);
                if (existing == null) return false;

                existing.Nom = request.Nom;
                existing.Description = request.Description;
                existing.DateDebut = request.DateDebut;
                existing.DateFin = request.DateFin;
                if (request.TeamId.HasValue)
                    existing.TeamId = request.TeamId.Value;
                existing.LastEditAt = DateTime.Now;
                existing.LastEditBy = lastEditBy;

                return _projetDao.Update(existing);
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await Task.Run(() => _projetDao.Delete(id));
        }

        private ProjetDto MapToDto(Projet projet)
        {
            var team = _teamService.GetById(projet.TeamId);

            return new ProjetDto
            {
                Id = projet.Id,
                Nom = projet.Nom,
                Description = projet.Description,
                DateDebut = projet.DateDebut,
                DateFin = projet.DateFin,
                TeamId = projet.TeamId,
                TeamName = team?.Name ?? "Équipe inconnue",
                ManagerId = 0,
                ManagerName = "Manager inconnu",
                Type = "Equipe",
                CreatedAt = projet.CreatedAt,
                CreatedBy = projet.CreatedBy,
                LastEditAt = projet.LastEditAt,
                LastEditBy = projet.LastEditBy,
                Progress = 0
            };
        }
    }
}