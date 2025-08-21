using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    /// <summary>
    /// DTO pour la mise à jour du statut d'une tâche par un opérateur
    /// </summary>
    public class UpdateTaskStatusDto
    {
        /// <summary>
        /// Nouveau statut de la tâche
        /// Valeurs autorisées : "En attente", "En cours", "Terminé", "Bloqué"
        /// </summary>
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [StringLength(50, ErrorMessage = "Le statut ne peut pas dépasser 50 caractères")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Nom/ID de l'utilisateur qui effectue la mise à jour
        /// </summary>
        [StringLength(100, ErrorMessage = "Le nom de l'utilisateur ne peut pas dépasser 100 caractères")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Commentaire optionnel sur le changement de statut
        /// </summary>
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères")]
        public string? Comment { get; set; }
    }
}
