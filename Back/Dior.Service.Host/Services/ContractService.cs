using Dior.Library.DTO.Contract;

namespace Dior.Service.Host.Services
{
    public class ContractService
    {
        private readonly ILogger<ContractService> _logger;

        public ContractService(ILogger<ContractService> logger)
        {
            _logger = logger;
        }

        public Task<List<ContractDto>> GetAllAsync()
        {
            var contracts = new List<ContractDto>
            {
                new ContractDto
                {
                    Id = 1,
                    ContractNumber = "CT-2024-001",
                    Title = "Contrat Test",
                    Description = "Contrat de test",
                    UserId = 1,
                    UserName = "Admin System",
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(335),
                    Status = "Active",
                    Salary = 50000,
                    ContractType = "CDI",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System"
                }
            };
            return Task.FromResult(contracts);
        }

        public Task<List<ContractDto>> GetByUserIdAsync(int userId)
        {
            var contracts = new List<ContractDto>();
            if (userId == 1)
            {
                contracts.Add(new ContractDto
                {
                    Id = 1,
                    ContractNumber = "CT-2024-001",
                    Title = "Contrat Admin",
                    UserId = userId,
                    UserName = "Admin System",
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System"
                });
            }
            return Task.FromResult(contracts);
        }

        public Task<ContractDto?> GetByIdAsync(int id)
        {
            if (id == 1)
            {
                var contract = new ContractDto
                {
                    Id = 1,
                    ContractNumber = "CT-2024-001",
                    Title = "Contrat Test",
                    UserId = 1,
                    UserName = "Admin System",
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CreatedBy = "System"
                };
                return Task.FromResult<ContractDto?>(contract);
            }
            return Task.FromResult<ContractDto?>(null);
        }

        public Task<ContractDto> CreateAsync(CreateContractDto dto)
        {
            var contract = new ContractDto
            {
                Id = new Random().Next(1000, 9999),
                ContractNumber = dto.ContractNumber,
                Title = dto.Title,
                Description = dto.Description,
                UserId = dto.UserId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Salary = dto.Salary,
                ContractType = dto.ContractType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            return Task.FromResult(contract);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(true);
        }
    }
}