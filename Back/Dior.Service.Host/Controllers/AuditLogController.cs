using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dior.Library.DTO;
using Dior.Service.Host.Extensions;
using Dior.Library.Interfaces.DAOs;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly IDA_AuditLog _auditLogDao;

        public AuditLogController(IDA_AuditLog auditLogDao)
        {
            _auditLogDao = auditLogDao;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll()
        {
            var logs = await Task.Run(() => _auditLogDao.GetAllAuditLogs());
            var dtos = logs.Select(log => log.ToDto()).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLogDto>> GetById(int id)
        {
            var log = await Task.Run(() => _auditLogDao.GetAuditLogById(id));
            if (log == null)
                return NotFound();
            
            return Ok(log.ToDto());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> Search([FromQuery] int? userId, [FromQuery] string? action)
        {
            var logs = await Task.Run(() => _auditLogDao.GetAllAuditLogs());
            
            if (userId.HasValue)
                logs = logs.Where(l => l.UserId == userId.Value);
                
            if (!string.IsNullOrEmpty(action))
                logs = logs.Where(l => l.Action.Contains(action, StringComparison.OrdinalIgnoreCase));
                
            var dtos = logs.Select(log => log.ToDto()).ToList();
            return Ok(dtos);
        }
    }
}
