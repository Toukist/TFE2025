using Dior.Library.DTO.Payroll;
using Dior.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/payslips")]
    public class PayslipController : ControllerBase
    {
        private readonly IPayslipService _payslipService;
        private readonly ILogger<PayslipController> _logger;

        public PayslipController(IPayslipService payslipService, ILogger<PayslipController> logger)
        {
            _payslipService = payslipService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<PayslipDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PayslipDto>>> GetAll()
        {
            var list = await _payslipService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PayslipDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PayslipDto>> GetById(int id)
        {
            var item = await _payslipService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("user/{userId:long}")]
        [ProducesResponseType(typeof(List<PayslipDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PayslipDto>>> GetByUser(long userId)
        {
            var list = await _payslipService.GetByUserIdAsync(userId);
            return Ok(list);
        }

        [HttpGet("period/{year:int}/{month:int}")]
        [ProducesResponseType(typeof(List<PayslipDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PayslipDto>>> GetByPeriod(int year, int month)
        {
            var list = await _payslipService.GetByPeriodAsync(month, year);
            return Ok(list);
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin,RH")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> Generate([FromBody] GeneratePayslipRequest request)
        {
            var count = await _payslipService.GenerateAsync(request);
            return Ok(count);
        }

        [HttpPost("{id:int}/send")]
        [Authorize(Roles = "Admin,RH")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Send(int id)
        {
            var ok = await _payslipService.SendAsync(id);
            return Ok(ok);
        }

        [HttpPost("send-bulk")]
        [Authorize(Roles = "Admin,RH")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> SendBulk([FromBody] List<int> ids)
        {
            var ok = await _payslipService.SendBulkAsync(ids);
            return Ok(ok);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,RH")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var ok = await _payslipService.DeleteAsync(id);
            return Ok(ok);
        }
    }
}
