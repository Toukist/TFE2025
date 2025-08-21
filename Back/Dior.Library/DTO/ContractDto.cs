using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string? UserFullName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }

    public class CreateContractRequest
    {
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public string ContractType { get; set; } = "CDI";
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        public decimal Salary { get; set; }
        
        public string Currency { get; set; } = "EUR";
        public string PaymentFrequency { get; set; } = "Mensuel";
        
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }

    public class UpdateContractRequest
    {
        public string ContractType { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Salary { get; set; }
        public string? Currency { get; set; }
        public string? PaymentFrequency { get; set; }
        public string? Status { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }
}