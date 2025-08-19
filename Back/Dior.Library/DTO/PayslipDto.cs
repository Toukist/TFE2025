using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class PayslipDto
    {
        public int Id { get; set; }
        
        // Employé
        public long UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserTeamName { get; set; } = string.Empty;
        
        // Période
        public int Month { get; set; }
        public int Year { get; set; }
        public string Period => $"{Month:00}/{Year}";
        
        // Montants
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal Deductions { get; set; }
        public decimal Bonus { get; set; }
        
        // Document
        public string FileUrl { get; set; } = string.Empty;
        public bool IsSent { get; set; } = false;
        public DateTime? SentDate { get; set; }
        
        // Audit
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
    }

    public class GeneratePayslipRequest
    {
        [Required]
        public int Month { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        public List<long>? UserIds { get; set; }
        public int? TeamId { get; set; }
    }

    public class CreatePayslipRequest
    {
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public int Month { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        [Required]
        public decimal GrossSalary { get; set; }
        
        [Required]
        public decimal NetSalary { get; set; }
        
        public decimal Deductions { get; set; } = 0;
        public decimal Bonus { get; set; } = 0;
        
        public string FileUrl { get; set; } = string.Empty;
    }
}