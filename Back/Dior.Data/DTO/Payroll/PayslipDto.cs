using System;

namespace Dior.Library.DTO.Payroll
{
    /// <summary>
    /// DTO pour les fiches de paie
    /// </summary>
    public class PayslipDto
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserFullName { get; set; }
        public string? UserTeamName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deductions { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsValidated { get; set; }
        public DateTime? ValidatedAt { get; set; }
        public string? ValidatedBy { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer une fiche de paie
    /// </summary>
    public class CreatePayslipDto
    {
        public int UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deductions { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour une fiche de paie
    /// </summary>
    public class UpdatePayslipDto
    {
        public decimal? GrossSalary { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deductions { get; set; }
        public string? Notes { get; set; }
        public bool? IsValidated { get; set; }
    }
}