using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO pour la r�assignation d'une t�che � un autre op�rateur
    /// </summary>
    public class ReassignTaskDto
    {
        /// <summary>
        /// ID du nouvel utilisateur assign� � la t�che
        /// </summary>
        [Required(ErrorMessage = "L'ID du nouvel utilisateur assign� est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de l'utilisateur doit �tre sup�rieur � 0")]
        public int NewAssignedToUserId { get; set; }

        /// <summary>
        /// Nom/ID de l'utilisateur qui effectue la r�assignation (g�n�ralement un manager)
        /// </summary>
        [StringLength(100, ErrorMessage = "Le nom de l'utilisateur ne peut pas d�passer 100 caract�res")]
        public string? ReassignedBy { get; set; }

        /// <summary>
        /// Raison de la r�assignation
        /// </summary>
        [StringLength(500, ErrorMessage = "La raison ne peut pas d�passer 500 caract�res")]
        public string? Reason { get; set; }

        /// <summary>
        /// Indique si l'ancien assign� doit �tre notifi�
        /// </summary>
        public bool NotifyPreviousAssignee { get; set; } = true;

        /// <summary>
        /// Indique si le nouvel assign� doit �tre notifi�
        /// </summary>
        public bool NotifyNewAssignee { get; set; } = true;
    }
}
