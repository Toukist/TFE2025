using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    /// <summary>
    /// DTO pour la création d'une nouvelle tâche avec validation
    /// </summary>
    public class CreateTaskDto
    {
        /// <summary>
        /// Titre de la tâche
        /// </summary>
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Le titre doit contenir entre 3 et 255 caractères")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description détaillée de la tâche
        /// </summary>
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères")]
        public string? Description { get; set; }

        /// <summary>
        /// ID de l'utilisateur à qui assigner la tâche
        /// </summary>
        [Required(ErrorMessage = "L'assignation est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de l'utilisateur assigné doit être supérieur à 0")]
        public int AssignedToUserId { get; set; }

        /// <summary>
        /// ID de l'utilisateur créateur de la tâche
        /// </summary>
        [Required(ErrorMessage = "Le créateur est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID du créateur doit être supérieur à 0")]
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// Date d'échéance optionnelle
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Priorité de la tâche
        /// </summary>
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        /// <summary>
        /// Catégorie de la tâche
        /// </summary>
        [StringLength(50, ErrorMessage = "La catégorie ne peut pas dépasser 50 caractères")]
        public string? Category { get; set; }

        /// <summary>
        /// Tags associés à la tâche (séparés par des virgules)
        /// </summary>
        [StringLength(200, ErrorMessage = "Les tags ne peuvent pas dépasser 200 caractères")]
        public string? Tags { get; set; }

        /// <summary>
        /// Indique si une notification doit être envoyée à l'assigné
        /// </summary>
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// Énumération pour les priorités de tâche
    /// </summary>
    public enum TaskPriority
    {
        Faible = 1,
        Normal = 2,
        Haute = 3,
        Urgente = 4
    }
}
