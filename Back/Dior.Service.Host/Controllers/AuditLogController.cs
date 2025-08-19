using Dior.Library.Interfaces.UserInterface.Services;
using Dior.Service.Host.Extensions;
using Dior.Service.Host.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
        {
            var logs = await _auditLogService.GetAllAsync();
            return Ok(logs.Select(log => log.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLogDto>> GetById(int id)
        {
            var log = await _auditLogService.GetByIdAsync(id);
            if (log == null)
                return NotFound();

            return Ok(log.ToDto());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> Search([FromQuery] int? userId, [FromQuery] string? action)
        {
            var logs = await _auditLogService.SearchAsync(userId, action);
            return Ok(logs.Select(log => log.ToDto()));
        }
    }
}
