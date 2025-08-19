namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO pour les crit�res de filtrage des t�ches
    /// </summary>
    public class TaskFilterDto
    {
        /// <summary>
        /// Filtrer par utilisateur assign�
        /// </summary>
        public int? AssignedToUserId { get; set; }

        /// <summary>
        /// Filtrer par cr�ateur
        /// </summary>
        public int? CreatedByUserId { get; set; }

        /// <summary>
        /// Filtrer par statut
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filtrer par priorit�
        /// </summary>
        public TaskPriority? Priority { get; set; }

        /// <summary>
        /// Filtrer par cat�gorie
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Date de d�but pour le filtrage par p�riode
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Date de fin pour le filtrage par p�riode
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Inclure uniquement les t�ches en retard
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
        /// Pagination : num�ro de page
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Pagination : nombre d'�l�ments par page
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
}
