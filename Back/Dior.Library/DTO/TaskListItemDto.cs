namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO pour l'affichage d'une tâche dans une liste avec informations enrichies
    /// </summary>
    public class TaskListItemDto
    {
        /// <summary>
        /// Identifiant de la tâche
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titre de la tâche
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description courte de la tâche (tronquée si nécessaire)
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Statut actuel de la tâche
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Informations sur l'utilisateur assigné
        /// </summary>
        public UserSummaryDto? AssignedToUser { get; set; }

        /// <summary>
        /// Informations sur le créateur de la tâche
        /// </summary>
        public UserSummaryDto? CreatedByUser { get; set; }

        /// <summary>
        /// Date de création
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date d'échéance
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Dernière modification
        /// </summary>
        public DateTime? LastEditAt { get; set; }

        /// <summary>
        /// Indique si la tâche est en retard
        /// </summary>
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && Status != "Terminé";

        /// <summary>
        /// Nombre de jours restants avant l'échéance
        /// </summary>
        public int? DaysUntilDue => DueDate?.Subtract(DateTime.Now).Days;

        /// <summary>
        /// Priorité de la tâche
        /// </summary>
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        /// <summary>
        /// Catégorie de la tâche
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Couleur CSS associée au statut pour l'affichage
        /// </summary>
        public string StatusColor => Status switch
        {
            "En attente" => "#fbbf24", // Jaune
            "En cours" => "#3b82f6",   // Bleu
            "Terminé" => "#10b981",    // Vert
            "Bloqué" => "#ef4444",     // Rouge
            _ => "#6b7280"             // Gris par défaut
        };
    }
}
