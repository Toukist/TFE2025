using System;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Task
{
    /// <summary>
    /// DTO pour les tâches
    /// </summary>
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// DTO pour créer une tâche
    /// </summary>
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [RegularExpression("^(En attente|En cours|Terminée|Annulée)$", ErrorMessage = "Statut invalide")]
        public string Status { get; set; } = "En attente";
        
        [Required(ErrorMessage = "L'utilisateur assigné est obligatoire")]
        public int AssignedToUserId { get; set; }
        
        [Required(ErrorMessage = "L'utilisateur créateur est obligatoire")]
        public int CreatedByUserId { get; set; }
        
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour une tâche
    /// </summary>
    public class UpdateTaskDto
    {
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string? Title { get; set; }
        
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }
        
        [RegularExpression("^(En attente|En cours|Terminée|Annulée)$", ErrorMessage = "Statut invalide")]
        public string? Status { get; set; }
        
        public int? AssignedToUserId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// DTO pour filtrer les tâches
    /// </summary>
    public class TaskFilterDto
    {
        public string? Status { get; set; }
        public int? AssignedToUserId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? DueBefore { get; set; }
    }

    /// <summary>
    /// DTO résumé pour affichage en liste
    /// </summary>
    public class TaskSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AssignedToUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}