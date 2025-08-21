using Dior.Library.DTO.Project;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Services
{
    public class ProjetService
    {
        private readonly DiorDbContext _context;
        private readonly ILogger<ProjetService> _logger;

        public ProjetService(DiorDbContext context, ILogger<ProjetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<ProjetDto>> GetAllAsync()
        {
            // Version mockée temporaire
            var projets = new List<ProjetDto>
            {
                new ProjetDto
                {
                    Id = 1,
                    Name = "Projet Test",
                    Description = "Description du projet test",
                    Status = "En cours",
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    ManagerId = 1,
                    ManagerName = "Admin System",
                    Budget = 10000,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System"
                }
            };
            return Task.FromResult(projets);
        }

        public Task<ProjetDto?> GetByIdAsync(int id)
        {
            if (id == 1)
            {
                var projet = new ProjetDto
                {
                    Id = 1,
                    Name = "Projet Test",
                    Description = "Description du projet test",
                    Status = "En cours",
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    ManagerId = 1,
                    ManagerName = "Admin System",
                    Budget = 10000,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System"
                };
                return Task.FromResult<ProjetDto?>(projet);
            }
            return Task.FromResult<ProjetDto?>(null);
        }

        public Task<List<ProjetDto>> GetByTeamIdAsync(int teamId)
        {
            // Version mockée
            var projets = new List<ProjetDto>();
            if (teamId == 1)
            {
                projets.Add(new ProjetDto
                {
                    Id = 1,
                    Name = "Projet Équipe 1",
                    Description = "Projet de l'équipe 1",
                    Status = "En cours",
                    StartDate = DateTime.UtcNow.AddDays(-15),
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    CreatedBy = "System"
                });
            }
            return Task.FromResult(projets);
        }

        public Task<ProjetDto> CreateAsync(CreateProjetDto createDto)
        {
            var projet = new ProjetDto
            {
                Id = new Random().Next(1000, 9999),
                Name = createDto.Name,
                Description = createDto.Description ?? string.Empty,
                Status = createDto.Status,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                ManagerId = createDto.ManagerId,
                Budget = createDto.Budget,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            return Task.FromResult(projet);
        }

        public Task<bool> UpdateAsync(int id, UpdateProjetDto updateDto)
        {
            // Version mockée - retourne toujours true
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int id)
        {
            // Version mockée - retourne toujours true
            return Task.FromResult(true);
        }

        private static ProjetDto MapToDto(Dior.Library.Entities.Team team)
        {
            // Conversion temporaire - devra être adapté avec une vraie entité Projet
            return new ProjetDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description ?? string.Empty,
                Status = "En cours",
                CreatedAt = team.CreatedAt,
                CreatedBy = team.CreatedBy ?? string.Empty
            };
        }
    }
}