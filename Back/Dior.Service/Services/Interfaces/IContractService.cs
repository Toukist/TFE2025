using Dior.Library.DTO;

namespace Dior.Service.Services.Interfaces
{
    public interface IContractService
    {
        Task<List<ContractDto>> GetAllAsync();
        Task<ContractDto?> GetByIdAsync(int id);
        Task<List<ContractDto>> GetByUserIdAsync(long userId);
        Task<List<ContractDto>> GetActiveContractsAsync();
        Task<ContractDto> CreateAsync(CreateContractRequest request);
        Task<bool> UpdateAsync(int id, UpdateContractRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> TerminateAsync(int id, DateTime endDate);
    }
}