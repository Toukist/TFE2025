using Dior.Library.DTO.Team;
using Dior.Library.DTO.User;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/team")]
    public class TeamController : ControllerBase
    {
        private readonly TeamService _teamService;
        private readonly UserService _userService;
        private readonly ILogger<TeamController> _logger;

        public TeamController(TeamService teamService, UserService userService, ILogger<TeamController> logger)
        {
            _teamService = teamService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamDto>>> GetAll()
        {
            var teams = await _teamService.GetAllAsync();
            return Ok(teams);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeamDto>> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }

        [HttpGet("{id:int}/users")]
        public async Task<ActionResult<List<UserDto>>> GetTeamUsers(int id)
        {
            var users = await _teamService.GetTeamMembersAsync(id);
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<TeamDto>> Create([FromBody] CreateTeamDto dto)
        {
            var team = await _teamService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamDto dto)
        {
            var exists = await _teamService.ExistsAsync(id);
            if (!exists) return NotFound();

            await _teamService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _teamService.ExistsAsync(id);
            if (!exists) return NotFound();

            await _teamService.DeleteAsync(id);
            return NoContent();
        }
    }
}