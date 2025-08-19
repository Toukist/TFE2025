using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserAccessCompetencyController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserAccessCompetencyController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/UserAccessCompetency/list?userId=123
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<UserAccessCompetencyDto>>> GetList([FromQuery] int userId)
    {
        var result = await _context.UserAccessCompetencies
            .Where(uac => uac.UserId == userId)
            .Join(_context.AccessCompetencies,
                  uac => uac.AccessCompetencyId,
                  ac => ac.Id,
                  (uac, ac) => new UserAccessCompetencyDto
                  {
                      Id = uac.Id,
                      UserId = uac.UserId,
                      AccessCompetencyId = uac.AccessCompetencyId,
                      Name = ac.Name,
                      IsActive = ac.IsActive,
                      CreatedAt = uac.CreatedAt,
                      CreatedBy = uac.CreatedBy,
                      LastEditAt = uac.LastEditAt,
                      LastEditBy = uac.LastEditBy
                  })
            .ToListAsync();

        return Ok(result);
    }

    // POST: api/UserAccessCompetency
    [HttpPost]
    public async Task<ActionResult<UserAccessCompetencyDto>> Post([FromBody] UserAccessCompetencyDto dto)
    {
        if (dto == null)
            return BadRequest("Données manquantes.");

        // Vérifie l'existence d'une entrée similaire
        var exists = await _context.UserAccessCompetencies
            .AnyAsync(x => x.UserId == dto.UserId && x.AccessCompetencyId == dto.AccessCompetencyId);

        if (exists)
            return Conflict("Cette compétence est déjà attribuée à cet utilisateur.");

        var entity = new UserAccessCompetency
        {
            UserId = dto.UserId,
            AccessCompetencyId = dto.AccessCompetencyId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "system"
        };

        _context.UserAccessCompetencies.Add(entity);
        await _context.SaveChangesAsync();

        // Récupère les infos du DTO à retourner
        var ac = await _context.AccessCompetencies.FindAsync(entity.AccessCompetencyId);

        var result = new UserAccessCompetencyDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccessCompetencyId = entity.AccessCompetencyId,
            Name = ac?.Name,
            IsActive = ac?.IsActive ?? false,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy
        };

        return CreatedAtAction(nameof(GetList), new { userId = entity.UserId }, result);
    }

    // DELETE: api/UserAccessCompetency/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.UserAccessCompetencies.FindAsync(id);
        if (entity == null)
            return NotFound();

        _context.UserAccessCompetencies.Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}