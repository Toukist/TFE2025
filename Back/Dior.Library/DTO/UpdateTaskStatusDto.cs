using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    /// <summary>
    /// DTO pour la mise � jour du statut d'une t�che par un op�rateur
    /// </summary>
    public class UpdateTaskStatusDto
    {
        /// <summary>
        /// Nouveau statut de la t�che
        /// Valeurs autoris�es : "En attente", "En cours", "Termin�", "Bloqu�"
        /// </summary>
        [Required(ErrorMessage = "Le statut est obligatoire")]
        [StringLength(50, ErrorMessage = "Le statut ne peut pas d�passer 50 caract�res")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Nom/ID de l'utilisateur qui effectue la mise � jour
        /// </summary>
        [StringLength(100, ErrorMessage = "Le nom de l'utilisateur ne peut pas d�passer 100 caract�res")]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Commentaire optionnel sur le changement de statut
        /// </summary>
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas d�passer 500 caract�res")]
        public string? Comment { get; set; }
    }
}
