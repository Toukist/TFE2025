using System;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Project
{
    /// <summary>
    /// DTO pour les projets
    /// </summary>
    public class ProjetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public decimal? Budget { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour créer un projet
    /// </summary>
    public class CreateProjetDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public string Status { get; set; } = "En cours";
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        public int? ManagerId { get; set; }
        public decimal? Budget { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour un projet
    /// </summary>
    public class UpdateProjetDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ManagerId { get; set; }
        public decimal? Budget { get; set; }
    }
}