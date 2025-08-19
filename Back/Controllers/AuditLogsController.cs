using Dior.Database.DTOs.AuditLog;
using Dior.Database.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dior.Database.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _service;

        public AuditLogsController(IAuditLogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Récupère tous les journaux d'audit
        /// </summary>
        /// <returns>Liste des journaux d'audit</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
        {
            var logs = await _service.GetAllAsync();
            return Ok(logs);
        }

        /// <summary>
        /// Récupère un journal d'audit par son ID
        /// </summary>
        /// <param name="id">ID du journal d'audit</param>
        /// <returns>Journal d'audit trouvé</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuditLogDto>> Get(int id)
        {
            var log = await _service.GetByIdAsync(id);
            if (log == null)
                return NotFound();

            return Ok(log);
        }

        /// <summary>
        /// Récupère les journaux d'audit par utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <returns>Liste des journaux d'audit de l'utilisateur</returns>
        [HttpGet("byUser/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetByUserId(int userId)
        {
            var logs = await _service.GetByUserIdAsync(userId);
            return Ok(logs);
        }

        /// <summary>
        /// Récupère les journaux d'audit par table
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <returns>Liste des journaux d'audit de la table</returns>
        [HttpGet("byTable/{tableName}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetByTableName(string tableName)
        {
            var logs = await _service.GetByTableNameAsync(tableName);
            return Ok(logs);
        }

        /// <summary>
        /// Crée un nouveau journal d'audit
        /// </summary>
        /// <param name="createDto">Données du journal d'audit</param>
        /// <returns>Nouveau journal d'audit créé</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AuditLogDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuditLogDto>> Create([FromBody] CreateAuditLogDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(createDto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }
    }
}