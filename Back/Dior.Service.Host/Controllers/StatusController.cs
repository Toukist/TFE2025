using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dior.Service.Services;
using Dior.Library.DTO;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IProjetService? _projetService;
        private readonly ITeamService? _teamService;

        public StatusController(IProjetService? projetService = null, ITeamService? teamService = null)
        {
            _projetService = projetService;
            _teamService = teamService;
        }

        /// <summary>
        /// GET /api/Status - Vérifie le status de l'API et des nouveaux endpoints
        /// </summary>
        [HttpGet]
        public ActionResult<object> GetStatus()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "2.0.0-refactored",
                message = "API Dior - Refactoring Phase 1 Complete",
                services = new
                {
                    projetService = _projetService != null ? "? Available" : "? Not injected",
                    teamService = _teamService != null ? "? Available" : "? Not injected"
                },
                availableEndpoints = new
                {
                    projets = new[]
                    {
                        "GET /api/Projet - Tous les projets",
                        "GET /api/Projet/{id} - Projet par ID", 
                        "GET /api/Projet/team/{teamId} - Projets d'une équipe",
                        "POST /api/Projet - Créer projet",
                        "PUT /api/Projet/{id} - Modifier projet",
                        "DELETE /api/Projet/{id} - Supprimer projet"
                    },
                    teams = new[]
                    {
                        "GET /api/Team - Toutes les équipes",
                        "GET /api/Team/{id} - Équipe par ID",
                        "GET /api/Team/{id}/users - Membres équipe (CRITIQUE pour Angular)",
                        "POST /api/Team - Créer équipe",
                        "PUT /api/Team/{id} - Modifier équipe",
                        "DELETE /api/Team/{id} - Supprimer équipe"
                    },
                    notifications = new[]
                    {
                        "GET /api/Notification/user/{userId} - Toutes notifications",
                        "GET /api/Notification/user/{userId}/unread - Non lues",
                        "PATCH /api/Notification/{id}/read - Marquer lu",
                        "PATCH /api/Notification/user/{userId}/read-all - Tout marquer lu",
                        "POST /api/Notification - Créer notification",
                        "POST /api/Notification/bulk - Notifications lot"
                    },
                    tasks = new[]
                    {
                        "GET /api/Task - Toutes les tâches",
                        "GET /api/Task/user/{userId} - Tâches utilisateur",
                        "GET /api/Task/status/{status} - Tâches par statut",
                        "PATCH /api/Task/{id}/status - Changer statut",
                        "PATCH /api/Task/{id}/assign - Réassigner"
                    }
                },
                refactoringStatus = new
                {
                    phase1Complete = true,
                    readyForAngularIntegration = true,
                    nextSteps = new[]
                    {
                        "Tests Postman des nouveaux endpoints",
                        "Intégration Angular frontend",
                        "Phase 2: AuditLog et Contract services"
                    }
                }
            });
        }

        /// <summary>
        /// GET /api/Status/endpoints - Liste détaillée des endpoints disponibles
        /// </summary>
        [HttpGet("endpoints")]
        public ActionResult<object> GetEndpoints()
        {
            return Ok(new
            {
                baseUrl = "https://localhost:7201",
                swagger = "https://localhost:7201/swagger",
                healthCheck = "https://localhost:7201/health",
                newEndpoints = new
                {
                    projets = "https://localhost:7201/api/Projet",
                    teams = "https://localhost:7201/api/Team", 
                    notifications = "https://localhost:7201/api/Notification",
                    tasks = "https://localhost:7201/api/Task"
                },
                testExamples = new
                {
                    getAllTeams = "GET https://localhost:7201/api/Team",
                    getTeamMembers = "GET https://localhost:7201/api/Team/1/users",
                    createProject = new
                    {
                        method = "POST",
                        url = "https://localhost:7201/api/Projet",
                        body = new
                        {
                            nom = "Nouveau Projet",
                            description = "Description du projet", 
                            teamId = 1,
                            dateDebut = "2025-01-20",
                            dateFin = "2025-06-30"
                        }
                    }
                }
            });
        }

        /// <summary>
        /// GET /api/Status/test-services - Test les services si disponibles
        /// </summary>
        [HttpGet("test-services")]
        public async Task<ActionResult<object>> TestServices()
        {
            var results = new
            {
                timestamp = DateTime.UtcNow,
                tests = new List<object>()
            };

            if (_teamService != null)
            {
                try
                {
                    var teams = await Task.Run(() => _teamService.GetAll());
                    ((List<object>)results.tests).Add(new
                    {
                        service = "TeamService",
                        status = "? Success",
                        result = $"Found {teams.Count} teams"
                    });
                }
                catch (Exception ex)
                {
                    ((List<object>)results.tests).Add(new
                    {
                        service = "TeamService",
                        status = "? Error",
                        error = ex.Message
                    });
                }
            }

            if (_projetService != null)
            {
                try
                {
                    var projets = await _projetService.GetAllAsync();
                    ((List<object>)results.tests).Add(new
                    {
                        service = "ProjetService",
                        status = "? Success",
                        result = $"Found {projets.Count} projets"
                    });
                }
                catch (Exception ex)
                {
                    ((List<object>)results.tests).Add(new
                    {
                        service = "ProjetService", 
                        status = "? Error",
                        error = ex.Message
                    });
                }
            }

            return Ok(results);
        }
    }
}