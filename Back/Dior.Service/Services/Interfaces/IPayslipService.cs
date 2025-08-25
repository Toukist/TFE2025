using Dior.Library.DTO.Payroll;

namespace Dior.Service.Services.Interfaces
{
    public interface IPayslipService
    {
        Task<List<PayslipDto>> GetAllAsync();
        Task<PayslipDto?> GetByIdAsync(int id);
        Task<List<PayslipDto>> GetByUserIdAsync(long userId);
        Task<List<PayslipDto>> GetByPeriodAsync(int month, int year);
        Task<int> GenerateAsync(GeneratePayslipRequest request);
        Task<bool> SendAsync(int payslipId);
        Task<bool> SendBulkAsync(List<int> payslipIds);
        Task<bool> DeleteAsync(int id);
    }
}