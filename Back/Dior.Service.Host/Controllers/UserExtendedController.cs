using Dior.Library.DTO;
using Dior.Library.Interfaces.DAOs;
using Dior.Service.Services;
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
        private readonly IDA_User _userDao;

        public UserExtendedController(ITeamService teamService, IDA_User userDao)
        {
            _teamService = teamService;
            _userDao = userDao;
        }

        /// <summary>
        /// GET api/UserExtended/{id}/full - Retourne les informations compl�tes d'un utilisateur
        /// Inclut: user, team, roles, acc�s comp�tences
        /// </summary>
        [HttpGet("{id}/full")]
        public async Task<ActionResult<UserFullDto>> GetFullInfo(long id)
        {
            // V�rifier les permissions - un utilisateur ne peut voir que ses propres infos sauf s'il est admin
            var currentUserIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("admin") || User.IsInRole("manager");
            
            if (!isAdmin && (!long.TryParse(currentUserIdClaim, out var currentUserId) || currentUserId != id))
            {
                return Forbid("Vous ne pouvez consulter que vos propres informations compl�tes");
            }

            try
            {
                // R�cup�rer l'utilisateur de base
                var user = await Task.Run(() => _userDao.GetUserById(id));
                if (user == null)
                    return NotFound($"Utilisateur avec l'ID {id} non trouv�");

                // R�cup�rer les informations de l'�quipe
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
                    Username = user.Username,
                    BadgePhysicalNumber = null, // TODO: r�cup�rer depuis UserAccess si n�cessaire
                    
                    // Informations �tendues de base
                    Roles = new List<string>() // TODO: impl�menter r�cup�ration des r�les
                };

                return Ok(userFull);
            }
            catch (Exception ex)
            {
                // Log l'erreur mais ne pas exposer les d�tails
                return StatusCode(500, new { message = "Erreur lors de la r�cup�ration des informations utilisateur" });
            }
        }

        /// <summary>
        /// GET api/UserExtended/{id}/summary - Retourne un r�sum� des informations utilisateur
        /// </summary>
        [HttpGet("{id}/summary")]
        public async Task<ActionResult<UserSummaryDto>> GetSummary(long id)
        {
            try
            {
                var user = await Task.Run(() => _userDao.GetUserById(id));
                if (user == null)
                    return NotFound($"Utilisateur avec l'ID {id} non trouv�");

                var summary = new UserSummaryDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    IsActive = user.IsActive,
                    TeamName = user.TeamId.HasValue ? 
                        _teamService.GetById(user.TeamId.Value)?.Name ?? "�quipe inconnue" : 
                        "Aucune �quipe"
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la r�cup�ration du r�sum� utilisateur" });
            }
        }
    }
}