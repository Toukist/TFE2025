using Dior.Database.DTOs.User;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// R�cup�re tous les utilisateurs
        /// </summary>
        /// <returns>Liste des utilisateurs</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// R�cup�re un utilisateur par son ID
        /// </summary>
        /// <param name="id">ID de l'utilisateur</param>
        /// <returns>Utilisateur trouv�</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Cr�e un nouvel utilisateur
        /// </summary>
        /// <param name="createUserDto">Donn�es de l'utilisateur</param>
        /// <returns>Nouvel utilisateur cr��</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userService.CreateAsync(createUserDto);
            return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Met � jour un utilisateur existant
        /// </summary>
        /// <param name="id">ID de l'utilisateur</param>
        /// <param name="updateUserDto">Donn�es � modifier</param>
        /// <returns>R�sultat de la mise � jour</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateAsync(id, updateUserDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        /// <param name="id">ID de l'utilisateur</param>
        /// <returns>R�sultat de la suppression</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}