namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO pour l'affichage d'une t�che dans une liste avec informations enrichies
    /// </summary>
    public class TaskListItemDto
    {
        /// <summary>
        /// Identifiant de la t�che
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Titre de la t�che
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description courte de la t�che (tronqu�e si n�cessaire)
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Statut actuel de la t�che
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Informations sur l'utilisateur assign�
        /// </summary>
        public UserSummaryDto? AssignedToUser { get; set; }

        /// <summary>
        /// Informations sur le cr�ateur de la t�che
        /// </summary>
        public UserSummaryDto? CreatedByUser { get; set; }

        /// <summary>
        /// Date de cr�ation
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date d'�ch�ance
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Derni�re modification
        /// </summary>
        public DateTime? LastEditAt { get; set; }

        /// <summary>
        /// Indique si la t�che est en retard
        /// </summary>
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && Status != "Termin�";

        /// <summary>
        /// Nombre de jours restants avant l'�ch�ance
        /// </summary>
        public int? DaysUntilDue => DueDate?.Subtract(DateTime.Now).Days;

        /// <summary>
        /// Priorit� de la t�che
        /// </summary>
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        /// <summary>
        /// Cat�gorie de la t�che
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Couleur CSS associ�e au statut pour l'affichage
        /// </summary>
        public string StatusColor => Status switch
        {
            "En attente" => "#fbbf24", // Jaune
            "En cours" => "#3b82f6",   // Bleu
            "Termin�" => "#10b981",    // Vert
            "Bloqu�" => "#ef4444",     // Rouge
            _ => "#6b7280"             // Gris par d�faut
        };
    }
}
