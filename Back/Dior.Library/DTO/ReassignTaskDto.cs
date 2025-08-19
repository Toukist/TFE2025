using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO pour la réassignation d'une tâche à un autre opérateur
    /// </summary>
    public class ReassignTaskDto
    {
        /// <summary>
        /// ID du nouvel utilisateur assigné à la tâche
        /// </summary>
        [Required(ErrorMessage = "L'ID du nouvel utilisateur assigné est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de l'utilisateur doit être supérieur à 0")]
        public int NewAssignedToUserId { get; set; }

        /// <summary>
        /// Nom/ID de l'utilisateur qui effectue la réassignation (généralement un manager)
        /// </summary>
        [StringLength(100, ErrorMessage = "Le nom de l'utilisateur ne peut pas dépasser 100 caractères")]
        public string? ReassignedBy { get; set; }

        /// <summary>
        /// Raison de la réassignation
        /// </summary>
        [StringLength(500, ErrorMessage = "La raison ne peut pas dépasser 500 caractères")]
        public string? Reason { get; set; }

        /// <summary>
        /// Indique si l'ancien assigné doit être notifié
        /// </summary>
        public bool NotifyPreviousAssignee { get; set; } = true;

        /// <summary>
        /// Indique si le nouvel assigné doit être notifié
        /// </summary>
        public bool NotifyNewAssignee { get; set; } = true;
    }
}
