using Dior.Library.DTO.Contract;
using Dior.Service.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/contract")]
    public class ContractController : ControllerBase
    {
        private readonly ContractService _contractService;
        private readonly ILogger<ContractController> _logger;

        public ContractController(ContractService contractService, ILogger<ContractController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,RH")]
        public async Task<ActionResult<List<ContractDto>>> GetAll()
        {
            var contracts = await _contractService.GetAllAsync();
            return Ok(contracts);
        }

        [HttpGet("user/{userId:int}")]
        [Authorize]
        public async Task<ActionResult<List<ContractDto>>> GetByUserId(int userId)
        {
            var contracts = await _contractService.GetByUserIdAsync(userId);
            return Ok(contracts);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ContractDto>> GetById(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,RH")]
        public async Task<ActionResult<ContractDto>> Upload([FromBody] CreateContractDto dto)
        {
            var contract = await _contractService.CreateAsync(dto);
            return Ok(contract);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,RH")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _contractService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}