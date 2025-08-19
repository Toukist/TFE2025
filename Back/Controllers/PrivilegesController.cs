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
        /// R�cup�re tous les privil�ges
        /// </summary>
        /// <returns>Liste des privil�ges</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PrivilegeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PrivilegeDto>>> GetAll()
        {
            var privileges = await _privilegeService.GetAllAsync();
            return Ok(privileges);
        }

        /// <summary>
        /// R�cup�re un privil�ge par son ID
        /// </summary>
        /// <param name="id">ID du privil�ge</param>
        /// <returns>Privil�ge trouv�</returns>
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
        /// Cr�e un nouveau privil�ge
        /// </summary>
        /// <param name="createPrivilegeDto">Donn�es du privil�ge</param>
        /// <returns>Nouveau privil�ge cr��</returns>
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
        /// Met � jour un privil�ge existant
        /// </summary>
        /// <param name="id">ID du privil�ge</param>
        /// <param name="updatePrivilegeDto">Donn�es � modifier</param>
        /// <returns>R�sultat de la mise � jour</returns>
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
        /// Supprime un privil�ge
        /// </summary>
        /// <param name="id">ID du privil�ge</param>
        /// <returns>R�sultat de la suppression</returns>
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