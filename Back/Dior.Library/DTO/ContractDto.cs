using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class ContractDto
    {
        public int Id { get; set; }
        
        // Employé
        public long UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserTeamName { get; set; } = string.Empty;
        
        // Détails contrat
        public string ContractType { get; set; } = "CDI"; // CDI, CDD, Stage, Alternance
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Null si CDI
        
        public decimal Salary { get; set; }
        public string Currency { get; set; } = "EUR";
        public string PaymentFrequency { get; set; } = "Mensuel";
        
        // Document
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        
        // Statut
        public string Status { get; set; } = "Actif"; // Actif, Expiré, Résilié
        
        // Audit
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }

        // Compatibility properties
        public DateTime UploadedAt => UploadDate;
        public bool IsFileAvailable => File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", FileUrl?.TrimStart('/') ?? string.Empty));
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