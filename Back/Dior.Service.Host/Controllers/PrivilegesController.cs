using Dior.Database.DTOs.Privilege;
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
    public class PrivilegesController : ControllerBase
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegesController(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
        }

        /// <summary>
        /// Récupère tous les privilèges
        /// </summary>
        /// <returns>Liste des privilèges</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PrivilegeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PrivilegeDto>>> GetAll()
        {
            var privileges = await _privilegeService.GetAllAsync();
            return Ok(privileges);
        }

        /// <summary>
        /// Récupère un privilège par son ID
        /// </summary>
        /// <param name="id">ID du privilège</param>
        /// <returns>Privilège trouvé</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PrivilegeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PrivilegeDto>> Get(int id)
        {
            var privilege = await _privilegeService.GetByIdAsync(id);
            if (privilege == null)
                return NotFound();

            return Ok(privilege);
        }

        /// <summary>
        /// Crée un nouveau privilège
        /// </summary>
        /// <param name="createPrivilegeDto">Données du privilège</param>
        /// <returns>Nouveau privilège créé</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PrivilegeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PrivilegeDto>> Create([FromBody] CreatePrivilegeDto createPrivilegeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdPrivilege = await _privilegeService.CreateAsync(createPrivilegeDto);
            return CreatedAtAction(nameof(Get), new { id = createdPrivilege.Id }, createdPrivilege);
        }

        /// <summary>
        /// Met à jour un privilège existant
        /// </summary>
        /// <param name="id">ID du privilège</param>
        /// <param name="updatePrivilegeDto">Données à modifier</param>
        /// <returns>Résultat de la mise à jour</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePrivilegeDto updatePrivilegeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _privilegeService.UpdateAsync(id, updatePrivilegeDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Supprime un privilège
        /// </summary>
        /// <param name="id">ID du privilège</param>
        /// <returns>Résultat de la suppression</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _privilegeService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}