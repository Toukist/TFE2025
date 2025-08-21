using System;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Contract
{
    /// <summary>
    /// DTO pour les contrats
    /// </summary>
    public class ContractDto
    {
        public int Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        public string? ContractType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer un contrat
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
    }

    /// <summary>
    /// DTO pour mettre à jour un contrat
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