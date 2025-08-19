using Dior.Library.DTO;
using Dior.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PayslipController : ControllerBase
    {
        private readonly IPayslipService _payslipService;
        
        public PayslipController(IPayslipService payslipService)
        {
            _payslipService = payslipService;
        }
        
        /// <summary>
        /// Générer et envoyer les fiches de paie (RH only)
        /// </summary>
        [HttpPost("generate")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult> GeneratePayslips([FromBody] GeneratePayslipRequest request)
        {
            // request contient: Month, Year, UserIds[] ou TeamId
            var count = await _payslipService.GenerateAsync(request);
            return Ok(new { message = $"{count} fiches de paie générées" });
        }
        
        /// <summary>
        /// Créer une fiche de paie individuelle
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult<PayslipDto>> Create([FromBody] CreatePayslipRequest request)
        {
            // Convert CreatePayslipRequest to GeneratePayslipRequest
            var generateRequest = new GeneratePayslipRequest
            {
                Month = request.Month,
                Year = request.Year,
                UserIds = new List<long> { request.UserId }
            };
            
            var count = await _payslipService.GenerateAsync(generateRequest);
            
            if (count > 0)
            {
                var payslips = await _payslipService.GetByUserIdAsync(request.UserId);
                var payslip = payslips.FirstOrDefault(p => p.Month == request.Month && p.Year == request.Year);
                return payslip != null ? Ok(payslip) : BadRequest("Erreur lors de la création");
            }
            
            return BadRequest("Impossible de créer la fiche de paie");
        }
        
        /// <summary>
        /// Liste toutes les fiches de paie (RH)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult<List<PayslipDto>>> GetAll()
        {
            var payslips = await _payslipService.GetAllAsync();
            return Ok(payslips);
        }
        
        /// <summary>
        /// Envoyer une fiche de paie
        /// </summary>
        [HttpPost("{id}/send")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult> SendPayslip(int id)
        {
            var sent = await _payslipService.SendAsync(id);
            return sent ? Ok() : BadRequest("Erreur lors de l'envoi");
        }
        
        /// <summary>
        /// Envoyer plusieurs fiches de paie
        /// </summary>
        [HttpPost("send-bulk")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult> SendBulkPayslips([FromBody] List<int> payslipIds)
        {
            var sent = await _payslipService.SendBulkAsync(payslipIds);
            return sent ? Ok() : BadRequest("Erreur lors de l'envoi groupé");
        }
        
        /// <summary>
        /// Mes fiches de paie (Employé)
        /// </summary>
        [HttpGet("my")]
        public async Task<ActionResult<List<PayslipDto>>> GetMyPayslips()
        {
            var userId = GetCurrentUserId();
            var payslips = await _payslipService.GetByUserIdAsync(userId);
            return Ok(payslips);
        }
        
        /// <summary>
        /// Fiches de paie d'un utilisateur (RH)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult<List<PayslipDto>>> GetByUserId(long userId)
        {
            var payslips = await _payslipService.GetByUserIdAsync(userId);
            return Ok(payslips);
        }
        
        /// <summary>
        /// Fiches de paie par période (RH)
        /// </summary>
        [HttpGet("period/{year}/{month}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult<List<PayslipDto>>> GetByPeriod(int year, int month)
        {
            var payslips = await _payslipService.GetByPeriodAsync(month, year);
            return Ok(payslips);
        }
        
        /// <summary>
        /// Obtenir une fiche de paie par ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PayslipDto>> GetById(int id)
        {
            var payslip = await _payslipService.GetByIdAsync(id);
            if (payslip == null) 
                return NotFound();

            // Vérifier les autorisations: RH/Admin peut voir toutes, employé seulement les siennes
            var currentUserId = GetCurrentUserId();
            var isHR = User.IsInRole("RH") || User.IsInRole("Admin");
            
            if (!isHR && payslip.UserId != currentUserId)
                return Forbid();
                
            return Ok(payslip);
        }
        
        /// <summary>
        /// Supprimer une fiche de paie
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "RH,Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _payslipService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        
        private long GetCurrentUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}