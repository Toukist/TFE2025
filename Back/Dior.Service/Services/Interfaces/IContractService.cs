using Dior.Library.DTO.Contract;

namespace Dior.Service.Services.Interfaces
{
    /// <summary>
    /// Service contrats (CRUD et actions)
    /// </summary>
    public interface IContractService
    {
        /// <summary>Retourne tous les contrats</summary>
        Task<List<ContractDto>> GetAllAsync();
<<<<<<< Updated upstream
        Task<ContractDto?> GetByIdAsync(int id);
        Task<List<ContractDto>> GetByUserIdAsync(int userId);
        Task<List<ContractDto>> GetActiveContractsAsync();
        Task<ContractDto> CreateAsync(CreateContractDto request);
        Task<bool> UpdateAsync(int id, UpdateContractDto request);
        Task<bool> DeleteAsync(int id);
        Task<bool> TerminateAsync(int id, DateTime endDate);
=======
        /// <summary>Retourne un contrat par id</summary>
        Task<ContractDto?> GetByIdAsync(long id);
        /// <summary>Retourne les contrats d'un utilisateur</summary>
        Task<List<ContractDto>> GetByUserIdAsync(long userId);
        /// <summary>Retourne les contrats actifs</summary>
        Task<List<ContractDto>> GetActiveContractsAsync();
        /// <summary>Crée un contrat</summary>
        Task<ContractDto> CreateAsync(CreateContractRequest request);
        /// <summary>Met à jour un contrat</summary>
        Task<bool> UpdateAsync(long id, UpdateContractRequest request);
        /// <summary>Supprime un contrat</summary>
        Task<bool> DeleteAsync(long id);
        /// <summary>Résilie un contrat</summary>
        Task<bool> TerminateAsync(long id, DateTime endDate);
>>>>>>> Stashed changes
    }
}