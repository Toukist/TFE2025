using Dior.Library.DTO;
using Dior.Service.Services;
using Dior.Service.DAO.UserInterfaces; // Pour DA_User
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authentification requise
    public class UserExtendedController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly DA_User _userDao;

        public UserExtendedController(ITeamService teamService, DA_User userDao)
        {
            _teamService = teamService;
            _userDao = userDao;
        }

        /// <summary>
        /// GET api/UserExtended/{id}/full - Retourne les informations complètes d'un utilisateur
        /// Inclut: user, team, roles, accès compétences
        /// </summary>
        [HttpGet("{id}/full")]
        public async Task<ActionResult<UserFullDto>> GetFullInfo(long id)
        {
            // Vérifier les permissions - un utilisateur ne peut voir que ses propres infos sauf s'il est admin
            var currentUserIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("admin") || User.IsInRole("manager");
            
            if (!isAdmin && (!long.TryParse(currentUserIdClaim, out var currentUserId) || currentUserId != id))
            {
                return Forbid("Vous ne pouvez consulter que vos propres informations complètes");
            }

            try
            {
                // Récupérer l'utilisateur de base
                var user = await Task.Run(() => _userDao.Get((int)id));
                if (user == null)
                    return NotFound($"Utilisateur avec l'ID {id} non trouvé");

                // Récupérer les informations de l'équipe
                TeamDto? team = null;
                if (user.TeamId.HasValue)
                {
                    var teamBo = await Task.Run(() => _teamService.GetById(user.TeamId.Value));
                    team = teamBo != null ? new TeamDto
                    {
                        Id = teamBo.Id,
                        Name = teamBo.Name,
                        Description = teamBo.Description,
                        CreatedAt = teamBo.CreatedAt
                    } : null;
                }

                // Récupérer les rôles de l'utilisateur
                var roles = await Task.Run(() => _userDao.GetUserRoles(id));

                // Construire le DTO complet
                var userFull = new UserFullDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    IsActive = user.IsActive,
                    TeamId = user.TeamId ?? 0,
                    TeamName = team?.Name ?? string.Empty,
                    Username = user.Name, // Legacy mapping
                    UserName = user.Name, // Nouveau mapping
                    BadgePhysicalNumber = null, // TODO: récupérer depuis UserAccess si nécessaire
                    
                    // Informations étendues de base
                    Roles = roles ?? new List<string>()
                };

                return Ok(userFull);
            }
            catch (Exception ex)
            {
                // Log l'erreur mais ne pas exposer les détails
                return StatusCode(500, new { message = "Erreur lors de la récupération des informations utilisateur" });
            }
        }

        /// <summary>
        /// GET api/UserExtended/{id}/summary - Retourne un résumé des informations utilisateur
        /// </summary>
        [HttpGet("{id}/summary")]
        public async Task<ActionResult<UserSummaryDto>> GetSummary(long id)
        {
            try
            {
                var user = await Task.Run(() => _userDao.Get((int)id));
                if (user == null)
                    return NotFound($"Utilisateur avec l'ID {id} non trouvé");

                var summary = new UserSummaryDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    IsActive = user.IsActive,
                    TeamName = user.TeamId.HasValue ? 
                        _teamService.GetById(user.TeamId.Value)?.Name ?? "Équipe inconnue" : 
                        "Aucune équipe"
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération du résumé utilisateur" });
            }
        }
    }
}