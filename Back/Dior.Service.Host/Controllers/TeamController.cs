// ----------------------------
// TeamController.cs
// ----------------------------
using Dior.Library.DTO;
using Dior.Service.DAO.UserInterfaces;
using Dior.Service.Services; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authentification requise
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _service;
        private readonly DA_User _userDao;

        public TeamController(ITeamService service, DA_User userDao)
        {
            _service = service;
            _userDao = userDao;
        }

        /// <summary>
        /// GET api/Team - Récupère toutes les équipes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TeamDto>>> GetAll()
        {
            var teams = await Task.Run(() => _service.GetAll().Select(MapToDto).ToList());
            return Ok(teams);
        }

        /// <summary>
        /// GET api/Team/{id} - Récupère une équipe par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetById(int id)
        {
            var team = await Task.Run(() => _service.GetById(id));
            if (team == null) 
                return NotFound($"Équipe avec l'ID {id} non trouvée");
            
            return Ok(MapToDto(team));
        }

        /// <summary>
        /// GET api/Team/{id}/users - CRITIQUE: Récupère les membres d'une équipe
        /// Endpoint essentiel pour Angular
        /// </summary>
        [HttpGet("{id}/users")]
        public async Task<ActionResult<List<UserDto>>> GetTeamMembers(int id)
        {
            // Vérifier que l'équipe existe
            var team = await Task.Run(() => _service.GetById(id));
            if (team == null)
                return NotFound($"Équipe avec l'ID {id} non trouvée");

            // Récupérer les utilisateurs de cette équipe
            var users = await Task.Run(() => 
                _userDao.GetAllUsersWithTeam().Where(u => u.TeamId == id).ToList());
            
            // Convert User entities to UserDto for the API response
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Phone = u.Phone,
                TeamId = u.TeamId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                CreatedBy = u.CreatedBy,
                LastEditAt = u.LastEditAt,
                LastEditBy = u.LastEditBy
            }).ToList();
            
            return Ok(userDtos);
        }

        /// <summary>
        /// POST api/Team - Crée une nouvelle équipe
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,manager")] // Seuls les admins et managers peuvent créer des équipes
        public async Task<ActionResult<TeamDto>> Create([FromBody] CreateTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var team = new Dior.Library.BO.Team
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.Now
            };

            await Task.Run(() => _service.Create(team));
            
            return CreatedAtAction(nameof(GetById), new { id = team.Id }, MapToDto(team));
        }

        /// <summary>
        /// PUT api/Team/{id} - Met à jour une équipe
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,manager")] // Seuls les admins et managers peuvent modifier des équipes
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await Task.Run(() => _service.GetById(id));
            if (existing == null) 
                return NotFound($"Équipe avec l'ID {id} non trouvée");

            var team = new Dior.Library.BO.Team
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = existing.CreatedAt // Préserver la date de création
            };

            await Task.Run(() => _service.Update(team));
            return NoContent();
        }

        /// <summary>
        /// DELETE api/Team/{id} - Supprime une équipe
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")] // Seuls les admins peuvent supprimer des équipes
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await Task.Run(() => _service.GetById(id));
            if (existing == null) 
                return NotFound($"Équipe avec l'ID {id} non trouvée");

            // Vérifier s'il y a des utilisateurs dans cette équipe
            var teamMembers = await Task.Run(() => 
                _userDao.GetAllUsersWithTeam().Where(u => u.TeamId == id).ToList());

            if (teamMembers.Any())
            {
                return BadRequest(new 
                { 
                    message = $"Impossible de supprimer l'équipe. {teamMembers.Count} utilisateur(s) y sont encore assigné(s).",
                    userCount = teamMembers.Count 
                });
            }

            await Task.Run(() => _service.Delete(id));
            return NoContent();
        }

        // ===== MÉTHODES DE MAPPING =====

        private static TeamDto MapToDto(Dior.Library.BO.Team bo) => new TeamDto
        {
            Id = bo.Id,
            Name = bo.Name,
            Description = bo.Description,
            CreatedAt = bo.CreatedAt
        };

        // ===== MÉTHODES LEGACY (conservées pour compatibilité) =====

        /// <summary>
        /// Legacy: Support pour les anciens DTOs
        /// </summary>
        [HttpPost]
        [Route("legacy")]
        [ApiExplorerSettings(IgnoreApi = true)] // Masquer dans Swagger
        public ActionResult<TeamDto> CreateLegacy([FromBody] TeamDto dto)
        {
            var team = MapToBo(dto);
            team.CreatedAt = DateTime.Now;
            _service.Create(team);
            return CreatedAtAction(nameof(GetById), new { id = team.Id }, MapToDto(team));
        }

        private static Dior.Library.BO.Team MapToBo(TeamDto dto) => new Dior.Library.BO.Team
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = dto.CreatedAt
        };
    }

    // ===== REQUEST/RESPONSE DTOs =====

    public class CreateTeamRequest
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le nom de l'équipe est obligatoire")]
        [System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
    }

    public class UpdateTeamRequest
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Le nom de l'équipe est obligatoire")]
        [System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
    }
}
