namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO pour les critères de filtrage des tâches
    /// </summary>
    public class TaskFilterDto
    {
        /// <summary>
        /// Filtrer par utilisateur assigné
        /// </summary>
        public int? AssignedToUserId { get; set; }

        /// <summary>
        /// Filtrer par créateur
        /// </summary>
        public int? CreatedByUserId { get; set; }

        /// <summary>
        /// Filtrer par statut
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filtrer par priorité
        /// </summary>
        public TaskPriority? Priority { get; set; }

        /// <summary>
        /// Filtrer par catégorie
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Date de début pour le filtrage par période
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Date de fin pour le filtrage par période
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Inclure uniquement les tâches en retard
        /// </summary>
        public bool? OnlyOverdue { get; set; }

        /// <summary>
        /// Recherche textuelle dans le titre et la description
        /// </summary>
        public string? SearchText { get; set; }

        /// <summary>
        /// Ordre de tri (CreatedAt, DueDate, Title, Status)
        /// </summary>
        public string OrderBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sens du tri (asc/desc)
        /// </summary>
        public string OrderDirection { get; set; } = "desc";

        /// <summary>
        /// Pagination : numéro de page
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Pagination : nombre d'éléments par page
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
