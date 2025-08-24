using Dior.Service.Services.UserInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/audit-log")]
    public class AuditLogController : ControllerBase
    {
        private readonly AuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(AuditLogService auditLogService, ILogger<AuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AuditLogDto>>> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var logs = await _auditLogService.GetLogsAsync(from, to);
            return Ok(logs);
        }

        [HttpGet("user/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AuditLogDto>>> GetUserLogs(int userId)
        {
            var logs = await _auditLogService.GetUserLogsAsync(userId);
            return Ok(logs);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<AuditLogDto>>> Search([FromQuery] string? action, [FromQuery] string? table)
        {
            var logs = await _auditLogService.SearchLogsAsync(action ?? string.Empty, table ?? string.Empty);
            return Ok(logs);
        }
    }
}