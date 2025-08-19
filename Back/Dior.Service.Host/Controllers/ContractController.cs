using Dior.Library.DTO;
using Dior.Service.Services;
using Dior.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly ContractService _service;
        private readonly IContractService? _contractService;
        
        public ContractController(ContractService service, IContractService? contractService = null)
        {
            _service = service;
            _contractService = contractService;
        }

        /// <summary>
        /// Liste tous les contrats (RH/Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "RH,Admin")]
        public async Task<IActionResult> GetAll()
        {
            if (_contractService != null)
            {
                var contracts = await _contractService.GetAllAsync();
                return Ok(contracts);
            }
            
            // Fallback to legacy service
            var legacyContracts = _service.GetAll();
            return Ok(legacyContracts);
        }

        /// <summary>
        /// Contrats d'un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<IActionResult> GetByUserId(long userId)
        {
            if (_contractService != null)
            {
                var contracts = await _contractService.GetByUserIdAsync(userId);
                return Ok(contracts);
            }
            
            // Fallback to legacy service
            var legacyContracts = _service.GetByUserId((int)userId);
            return Ok(legacyContracts);
        }

        /// <summary>
        /// Mes contrats (Employé)
        /// </summary>
        [HttpGet("my")]
        public async Task<ActionResult<List<ContractDto>>> GetMyContracts()
        {
            var userId = GetCurrentUserId();
            
            if (_contractService != null)
            {
                var contracts = await _contractService.GetByUserIdAsync(userId);
                return Ok(contracts);
            }
            
            // Fallback to legacy service
            var legacyContracts = _service.GetByUserId((int)userId);
            return Ok(legacyContracts);
        }

        /// <summary>
        /// Créer/uploader un contrat
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "RH,Admin")]
        public async Task<IActionResult> Upload([FromBody] ContractDto dto)
        {
            if (_contractService != null)
            {
                // Convert ContractDto to CreateContractRequest
                var request = new CreateContractRequest
                {
                    UserId = dto.UserId,
                    ContractType = dto.ContractType,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Salary = dto.Salary,
                    Currency = dto.Currency,
                    PaymentFrequency = dto.PaymentFrequency,
                    FileName = dto.FileName,
                    FileUrl = dto.FileUrl
                };
                
                var contract = await _contractService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
            }
            
            // Fallback to legacy service
            _service.Create(dto);
            return Ok();
        }

        /// <summary>
        /// Supprimer un contrat
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_contractService != null)
            {
                var deleted = await _contractService.DeleteAsync(id);
                return deleted ? NoContent() : NotFound();
            }
            
            // Fallback to legacy service
            _service.Delete(id);
            return NoContent();
        }

        /// <summary>
        /// Obtenir un contrat par ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            if (_contractService != null)
            {
                var contract = await _contractService.GetByIdAsync(id);
                return contract == null ? NotFound() : Ok(contract);
            }
            
            // Fallback to legacy service
            var legacyContract = _service.GetById(id);
            return legacyContract == null ? NotFound() : Ok(legacyContract);
        }

        /// <summary>
        /// Mettre à jour un contrat
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateContractRequest request)
        {
            if (_contractService != null)
            {
                var updated = await _contractService.UpdateAsync(id, request);
                return updated ? NoContent() : NotFound();
            }
            
            return BadRequest("Service de mise à jour non disponible");
        }

        /// <summary>
        /// Terminer un contrat
        /// </summary>
        [HttpPatch("{id}/terminate")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult> Terminate(int id, [FromBody] DateTime endDate)
        {
            if (_contractService != null)
            {
                var terminated = await _contractService.TerminateAsync(id, endDate);
                return terminated ? NoContent() : NotFound();
            }
            
            return BadRequest("Service de terminaison non disponible");
        }
        
        private long GetCurrentUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}
