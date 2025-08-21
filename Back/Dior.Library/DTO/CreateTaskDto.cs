using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTOs
{
    /// <summary>
    /// DTO pour la cr�ation d'une nouvelle t�che avec validation
    /// </summary>
    public class CreateTaskDto
    {
        /// <summary>
        /// Titre de la t�che
        /// </summary>
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Le titre doit contenir entre 3 et 255 caract�res")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description d�taill�e de la t�che
        /// </summary>
        [StringLength(1000, ErrorMessage = "La description ne peut pas d�passer 1000 caract�res")]
        public string? Description { get; set; }

        /// <summary>
        /// ID de l'utilisateur � qui assigner la t�che
        /// </summary>
        [Required(ErrorMessage = "L'assignation est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID de l'utilisateur assign� doit �tre sup�rieur � 0")]
        public int AssignedToUserId { get; set; }

        /// <summary>
        /// ID de l'utilisateur cr�ateur de la t�che
        /// </summary>
        [Required(ErrorMessage = "Le cr�ateur est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "L'ID du cr�ateur doit �tre sup�rieur � 0")]
        public int CreatedByUserId { get; set; }

        /// <summary>
        /// Date d'�ch�ance optionnelle
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Priorit� de la t�che
        /// </summary>
        public TaskPriority Priority { get; set; } = TaskPriority.Normal;

        /// <summary>
        /// Cat�gorie de la t�che
        /// </summary>
        [StringLength(50, ErrorMessage = "La cat�gorie ne peut pas d�passer 50 caract�res")]
        public string? Category { get; set; }

        /// <summary>
        /// Tags associ�s � la t�che (s�par�s par des virgules)
        /// </summary>
        [StringLength(200, ErrorMessage = "Les tags ne peuvent pas d�passer 200 caract�res")]
        public string? Tags { get; set; }

        /// <summary>
        /// Indique si une notification doit �tre envoy�e � l'assign�
        /// </summary>
        public bool SendNotification { get; set; } = true;
    }

    /// <summary>
    /// �num�ration pour les priorit�s de t�che
    /// </summary>
    public enum TaskPriority
    {
        Faible = 1,
        Normal = 2,
        Haute = 3,
        Urgente = 4
    }
}
