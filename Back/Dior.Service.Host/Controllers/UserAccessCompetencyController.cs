using Dior.Library;
using Dior.Library.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserAccessCompetencyController : ControllerBase
    {
        private readonly IDiorContext _context;

        public UserAccessCompetencyController(IDiorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère les compétences d'accès liées à un utilisateur.
        /// </summary>
        [HttpGet("list")]
        public ActionResult<List<UserAccessCompetencyDto>> GetListForUser([FromQuery] int userId)
        {
            var entities = _context.UserAccessCompetencies
                .Include(uac => uac.AccessCompetency)
                .Where(uac => uac.UserId == userId)
                .AsNoTracking()
                .ToList();

            if (!entities.Any())
                return NotFound("Aucune compétence trouvée pour cet utilisateur.");

            var result = entities.Select(e => new UserAccessCompetencyDto
            {
                Id = e.Id,
                UserId = e.UserId,
                AccessCompetencyId = e.AccessCompetencyId,
                Name = e.AccessCompetency?.Name ?? "",
                IsActive = e.AccessCompetency?.IsActive ?? false,
                CreatedAt = e.CreatedAt,
                CreatedBy = e.CreatedBy,
                LastEditAt = e.LastEditAt,
                LastEditBy = e.LastEditBy
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Ajoute une compétence d'accès à un utilisateur.
        /// </summary>
        [HttpPost]
        public IActionResult Create(UserAccessCompetencyDto dto)
        {
            var entity = new UserAccessCompetency
            {
                UserId = dto.UserId,
                AccessCompetencyId = dto.AccessCompetencyId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Environment.UserName,
                LastEditAt = DateTime.UtcNow,
                LastEditBy = Environment.UserName
            };

            _context.UserAccessCompetencies.Add(entity);
            (_context as DbContext)?.SaveChanges();

            return CreatedAtAction(nameof(GetListForUser), new { userId = dto.UserId }, dto);
        }

        /// <summary>
        /// Supprime une compétence d'accès utilisateur par ID.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _context.UserAccessCompetencies.FirstOrDefault(e => e.Id == id);
            if (entity == null)
                return NotFound();

            _context.UserAccessCompetencies.Remove(entity);
            (_context as DbContext)?.SaveChanges();

            return NoContent();
        }
    }
}
