using Dior.Library.DTO.Contract;

namespace Dior.Service.Services.Interfaces
{
    public interface IContractService
    {
        Task<List<ContractDto>> GetAllAsync();
        Task<ContractDto?> GetByIdAsync(int id);
        Task<List<ContractDto>> GetByUserIdAsync(int userId);
        Task<List<ContractDto>> GetActiveContractsAsync();
        Task<ContractDto> CreateAsync(CreateContractDto request);
        Task<bool> UpdateAsync(int id, UpdateContractDto request);
        Task<bool> DeleteAsync(int id);
        Task<bool> TerminateAsync(int id, DateTime endDate);
    }
}