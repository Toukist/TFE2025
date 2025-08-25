using Dior.Library.BO;
using Dior.Library.DTO.Payroll;
using Dior.Service.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Dior.Service.Services
{
    public class PayslipService : IPayslipService
    {
        private readonly string _connectionString;
        
        public PayslipService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Dior_DB") 
                ?? configuration.GetConnectionString("DIOR_DB")
                ?? throw new ArgumentException("Connection string manquante");
        }
        
        public async Task<List<PayslipDto>> GetAllAsync()
        {
            var payslips = new List<PayslipDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT p.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Payslips p
                LEFT JOIN [USER] u ON p.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                ORDER BY p.Year DESC, p.Month DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                payslips.Add(MapFromReader(reader));
            }
            
            return payslips;
        }
        
        public async Task<PayslipDto?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT p.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Payslips p
                LEFT JOIN [USER] u ON p.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                WHERE p.Id = @Id";
                
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
        
        public async Task<List<PayslipDto>> GetByUserIdAsync(long userId)
        {
            var payslips = new List<PayslipDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT p.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Payslips p
                LEFT JOIN [USER] u ON p.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                WHERE p.UserId = @UserId
                ORDER BY p.Year DESC, p.Month DESC";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                payslips.Add(MapFromReader(reader));
            }
            
            return payslips;
        }
        
        public async Task<List<PayslipDto>> GetByPeriodAsync(int month, int year)
        {
            var payslips = new List<PayslipDto>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                SELECT p.*, 
                       u.FirstName + ' ' + u.LastName as UserFullName,
                       t.Name as UserTeamName
                FROM Payslips p
                LEFT JOIN [USER] u ON p.UserId = u.ID
                LEFT JOIN Team t ON u.TeamId = t.Id
                WHERE p.Month = @Month AND p.Year = @Year
                ORDER BY u.LastName, u.FirstName";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Parameters.AddWithValue("@Year", year);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                payslips.Add(MapFromReader(reader));
            }
            
            return payslips;
        }
        
        public async Task<int> GenerateAsync(GeneratePayslipRequest request)
        {
            var userIds = new List<long>();
            
            // Déterminer les utilisateurs pour lesquels générer les fiches
            if (request.UserIds != null && request.UserIds.Count > 0)
            {
                userIds = request.UserIds;
            }
            else if (request.TeamId.HasValue)
            {
                // Récupérer tous les utilisateurs de l'équipe
                userIds = await GetUserIdsByTeamAsync(request.TeamId.Value);
            }
            else
            {
                throw new ArgumentException("UserIds ou TeamId requis pour la génération");
            }
            
            var generatedCount = 0;
            
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            
            foreach (var userId in userIds)
            {
                // Vérifier si la fiche existe déjà
                if (await PayslipExistsAsync(userId, request.Month, request.Year, conn))
                    continue;
                
                // Récupérer les infos du contrat pour calculer le salaire
                var contractInfo = await GetUserContractInfoAsync(userId, conn);
                if (!contractInfo.HasValue) continue;
                
                // Calculer les montants
                var grossSalary = contractInfo.Value.Salary;
                var deductions = CalculateDeductions(grossSalary);
                var netSalary = grossSalary - deductions;
                
                // Insérer la fiche de paie
                var query = @"
                    INSERT INTO Payslips (UserId, Month, Year, GrossSalary, NetSalary, Deductions, Bonus, FileUrl, IsSent, GeneratedAt, GeneratedBy)
                    VALUES (@UserId, @Month, @Year, @GrossSalary, @NetSalary, @Deductions, 0, '', 0, GETDATE(), 'System')";
                    
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Month", request.Month);
                cmd.Parameters.AddWithValue("@Year", request.Year);
                cmd.Parameters.AddWithValue("@GrossSalary", grossSalary);
                cmd.Parameters.AddWithValue("@NetSalary", netSalary);
                cmd.Parameters.AddWithValue("@Deductions", deductions);
                
                await cmd.ExecuteNonQueryAsync();
                generatedCount++;
            }
            
            return generatedCount;
        }
        
        public async Task<bool> SendAsync(int payslipId)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = @"
                UPDATE Payslips 
                SET IsSent = 1, SentDate = GETDATE() 
                WHERE Id = @PayslipId";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PayslipId", payslipId);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        public async Task<bool> SendBulkAsync(List<int> payslipIds)
        {
            if (!payslipIds.Any()) return false;
            
            using var conn = new SqlConnection(_connectionString);
            var idList = string.Join(",", payslipIds);
            var query = $@"
                UPDATE Payslips 
                SET IsSent = 1, SentDate = GETDATE() 
                WHERE Id IN ({idList})";
                
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            var query = "DELETE FROM Payslips WHERE Id = @Id";
            
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        
        // Helper methods
        private async Task<List<long>> GetUserIdsByTeamAsync(int teamId)
        {
            var userIds = new List<long>();
            
            using var conn = new SqlConnection(_connectionString);
            var query = "SELECT ID FROM [USER] WHERE TeamId = @TeamId AND IsActive = 1";
            
            conn.Open();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@TeamId", teamId);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                userIds.Add(reader.GetInt64(reader.GetOrdinal("ID")));
            }
            
            return userIds;
        }
        
        private async Task<bool> PayslipExistsAsync(long userId, int month, int year, SqlConnection conn)
        {
            var query = "SELECT COUNT(*) FROM Payslips WHERE UserId = @UserId AND Month = @Month AND Year = @Year";
            
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Month", month);
            cmd.Parameters.AddWithValue("@Year", year);
            
            var count = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(count) > 0;
        }
        
        private async Task<(decimal Salary, string Currency)?> GetUserContractInfoAsync(long userId, SqlConnection conn)
        {
            var query = @"
                SELECT Salary, Currency 
                FROM Contract 
                WHERE UserId = @UserId 
                  AND (Status = 'Actif' OR Status IS NULL)
                  AND (EndDate IS NULL OR EndDate > GETDATE())
                ORDER BY StartDate DESC";
                
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var salary = SafeGetDecimal(reader, "Salary") ?? 2000; // Default salary
                var currency = SafeGetString(reader, "Currency") ?? "EUR";
                return (salary, currency);
            }
            
            return (2000, "EUR"); // Default values if no contract found
        }
        
        private decimal CalculateDeductions(decimal grossSalary)
        {
            // Simple calculation: 25% deductions (social security, taxes, etc.)
            return Math.Round(grossSalary * 0.25m, 2);
        }

        private PayslipDto MapFromReader(SqlDataReader reader)
        {
            return new PayslipDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt64(reader.GetOrdinal("UserId")),
                UserFullName = SafeGetString(reader, "UserFullName") ?? string.Empty,
                UserTeamName = SafeGetString(reader, "UserTeamName") ?? string.Empty,
                Month = reader.GetInt32(reader.GetOrdinal("Month")),
                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                GrossSalary = reader.GetDecimal(reader.GetOrdinal("GrossSalary")),
                NetSalary = reader.GetDecimal(reader.GetOrdinal("NetSalary")),
                Deductions = SafeGetDecimal(reader, "Deductions") ?? 0,
                Bonus = SafeGetDecimal(reader, "Bonus") ?? 0,
                FileUrl = SafeGetString(reader, "FileUrl") ?? string.Empty,
                IsSent = reader.GetBoolean(reader.GetOrdinal("IsSent")),
                SentDate = SafeGetDateTime(reader, "SentDate"),
                GeneratedAt = reader.GetDateTime(reader.GetOrdinal("GeneratedAt")),
                GeneratedBy = SafeGetString(reader, "GeneratedBy") ?? string.Empty
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
    }
}