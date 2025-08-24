using System;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Contract
{
    /// <summary>
    /// DTO pour les contrats (lecture/retour API)
    /// </summary>
    public class ContractDto
    {
        // Identifiants
        public int Id { get; set; }
        public int UserId { get; set; }

        // M�tadonn�es utilisateur li�es (jointures)
        public string? UserFullName { get; set; }
        public string? UserTeamName { get; set; }

        // Informations de contrat (optionnelles selon source)
        public string? ContractNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public decimal? Salary { get; set; }
        public string? ContractType { get; set; }
        public string? Currency { get; set; }
        public string? PaymentFrequency { get; set; }

        // Fichier stock�
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }

        // Dates (toutes nullable)
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Audit
        public string? UploadedBy { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// DTO pour cr�er un contrat
    /// </summary>
    public class CreateContractDto
    {
        [Required]
        public string ContractNumber { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        public string Status { get; set; } = "Active";
        
        public decimal? Salary { get; set; }
        public string? ContractType { get; set; }

        // Champs optionnels utilis�s par ContractService
        public string? Currency { get; set; }
        public string? PaymentFrequency { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }

    /// <summary>
    /// DTO pour mettre � jour un contrat
    /// </summary>
    public class UpdateContractDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public decimal? Salary { get; set; }
        public string? ContractType { get; set; }
    }
}