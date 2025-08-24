using Dior.Library.BO;
using Dior.Library.DAO;
using Dior.Library.DTO.Contract;
using Dior.Service.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dior.Service.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractDao _dao;
        private readonly string _connectionString;

        public ContractService(IContractDao dao, IConfiguration configuration)
        {
            _dao = dao;
            _connectionString = configuration.GetConnectionString("Dior_DB")
                ?? configuration.GetConnectionString("DIOR_DB")
                ?? throw new ArgumentException("Connection string manquante");
        }

        // Legacy methods for compatibility
        public List<ContractDto> GetAll()
        {
            return _dao.GetAll().Select(MapToDto).ToList();
        }

        public List<ContractDto> GetByUserId(long userId)
        {
            return _dao.GetByUserId(userId).Select(MapToDto).ToList();
        }

        public ContractDto? GetById(long id)
        {
            var bo = _dao.GetById(id);
            return bo == null ? null : MapToDto(bo);
        }

        public void Create(ContractDto dto)
        {
            var bo = MapToBo(dto);
            bo.UploadedAt = DateTime.Now;
            _dao.Create(bo);
        }

        public void Delete(long id) => _dao.Delete(id);

        // New async methods implementing IContractService
        public async Task<List<ContractDto>> GetAllAsync()
        {
            var contracts = new List<ContractDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT c.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Contract c
                LEFT JOIN [USER] u ON c.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                ORDER BY c.UploadedAt DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                contracts.Add(MapFromReader(reader));
            }
            
            return contracts;
        }
        
        public async Task<ContractDto?> GetByIdAsync(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT c.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Contract c
                LEFT JOIN [USER] u ON c.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                WHERE c.Id = @Id";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapFromReader(reader);
            }
            
            return null;
        }
        
        public async Task<List<ContractDto>> GetByUserIdAsync(long userId)
        {
            var contracts = new List<ContractDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT c.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Contract c
                LEFT JOIN [USER] u ON c.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                WHERE c.UserId = @UserId
                ORDER BY c.UploadedAt DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(MapFromReader(reader));
            }
            
            return contracts;
        }
        
        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            var contracts = new List<ContractDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT c.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Contract c
                LEFT JOIN [USER] u ON c.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                WHERE (c.Status = 'Actif' OR c.Status IS NULL)
                  AND (c.EndDate IS NULL OR c.EndDate > GETDATE())
                ORDER BY c.UploadedAt DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                contracts.Add(MapFromReader(reader));
            }
            
            return contracts;
        }
        
        public async Task<ContractDto> CreateAsync(CreateContractRequest request)
        {
            using var conn = new SqlConnection(_connectionString);
            
            var query = @"
                INSERT INTO Contract (UserId, ContractType, StartDate, EndDate, Salary, Currency, PaymentFrequency, FileName, FileUrl, Status, UploadedAt, UploadedBy)
                OUTPUT INSERTED.Id
                VALUES (@UserId, @ContractType, @StartDate, @EndDate, @Salary, @Currency, @PaymentFrequency, @FileName, @FileUrl, 'Actif', GETDATE(), 'System')";
                
            try
            {
                conn.Open();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", request.UserId);
                cmd.Parameters.AddWithValue("@ContractType", request.ContractType ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", request.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", (object?)request.EndDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Salary", (object?)request.Salary ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Currency", (object?)request.Currency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PaymentFrequency", (object?)request.PaymentFrequency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FileName", request.FileName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FileUrl", request.FileUrl ?? (object)DBNull.Value);
                
                var newId = await cmd.ExecuteScalarAsync();
                var contractId = Convert.ToInt64(newId);
                
                return await GetByIdAsync(contractId) ?? throw new InvalidOperationException("Erreur lors de la création du contrat");
            }
            catch (SqlException)
            {
                var basicQuery = @"
                    INSERT INTO Contract (UserId, FileName, FileUrl, UploadedAt, UploadedBy)
                    OUTPUT INSERTED.Id
                    VALUES (@UserId, @FileName, @FileUrl, GETDATE(), 'System')";
                    
                using var basicCmd = new SqlCommand(basicQuery, conn);
                basicCmd.Parameters.AddWithValue("@UserId", request.UserId);
                basicCmd.Parameters.AddWithValue("@FileName", request.FileName ?? (object)DBNull.Value);
                basicCmd.Parameters.AddWithValue("@FileUrl", request.FileUrl ?? (object)DBNull.Value);
                
                var newId = await basicCmd.ExecuteScalarAsync();
                var contractId = Convert.ToInt64(newId);
                
                return await GetByIdAsync(contractId) ?? throw new InvalidOperationException("Erreur lors de la création du contrat");
            }
        }
        
        public async Task<bool> UpdateAsync(long id, UpdateContractRequest request)
        {
            var setClauses = new List<string>();
            var parameters = new List<SqlParameter>();
            
            if (!string.IsNullOrEmpty(request.ContractType))
            {
                setClauses.Add("ContractType = @ContractType");
                parameters.Add(new SqlParameter("@ContractType", request.ContractType));
            }
            
            if (request.StartDate.HasValue)
            {
                setClauses.Add("StartDate = @StartDate");
                parameters.Add(new SqlParameter("@StartDate", request.StartDate.Value));
            }
            
            if (request.Salary.HasValue)
            {
                setClauses.Add("Salary = @Salary");
                parameters.Add(new SqlParameter("@Salary", request.Salary.Value));
            }
            
            if (!string.IsNullOrEmpty(request.Status))
            {
                setClauses.Add("Status = @Status");
                parameters.Add(new SqlParameter("@Status", request.Status));
            }
            
            if (setClauses.Count == 0)
                return false;
                
            using var conn = new SqlConnection(_connectionString);
            var query = $"UPDATE Contract SET {string.Join(", ", setClauses)} WHERE Id = @Id";
            
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddRange(parameters.ToArray());
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = "DELETE FROM Contract WHERE Id = @Id";
            
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        public async Task<bool> TerminateAsync(long id, DateTime endDate)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                UPDATE Contract 
                SET EndDate = @EndDate, Status = 'Résilié'
                WHERE Id = @Id";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        private ContractDto MapToDto(ContractBO bo) => new ContractDto
        {
            Id = bo.Id,
            UserId = bo.UserId,
            FileName = bo.FileName,
            FileUrl = bo.FileUrl,
            UploadDate = bo.UploadedAt,
            UploadedBy = bo.UploadedBy
        };

        private ContractBO MapToBo(ContractDto dto) => new ContractBO
        {
            Id = dto.Id,
            UserId = dto.UserId,
            FileName = dto.FileName ?? string.Empty,
            FileUrl = dto.FileUrl ?? string.Empty,
            UploadedAt = dto.UploadDate ?? DateTime.UtcNow,
            UploadedBy = dto.UploadedBy ?? ""
        };
        
        private ContractDto MapFromReader(SqlDataReader reader)
        {
            return new ContractDto
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                UserFullName = SafeGetString(reader, "UserFullName") ?? string.Empty,
                UserTeamName = SafeGetString(reader, "UserTeamName") ?? string.Empty,
                ContractType = SafeGetString(reader, "ContractType") ?? "CDI",
                StartDate = SafeGetDateTime(reader, "StartDate"),
                EndDate = SafeGetDateTime(reader, "EndDate"),
                Salary = SafeGetDecimal(reader, "Salary"),
                Currency = SafeGetString(reader, "Currency") ?? "EUR",
                PaymentFrequency = SafeGetString(reader, "PaymentFrequency") ?? "Mensuel",
                FileName = SafeGetString(reader, "FileName"),
                FileUrl = SafeGetString(reader, "FileUrl"),
                Status = SafeGetString(reader, "Status") ?? "Actif",
                UploadDate = SafeGetDateTime(reader, "UploadedAt"),
                UploadedBy = SafeGetString(reader, "UploadedBy")
            };
        }
        
        private string? SafeGetString(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        private DateTime? SafeGetDateTime(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }
        
        private decimal? SafeGetDecimal(SqlDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
            }
            catch
            {
                return null;
            }
        }

        public Task<ContractDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, UpdateContractRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TerminateAsync(int id, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}