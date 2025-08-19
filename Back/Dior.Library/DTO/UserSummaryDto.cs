namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO contenant les informations essentielles d'un utilisateur pour l'affichage
    /// </summary>
    public class UserSummaryDto
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        public long Id { get; set; } // Changé en long pour cohérence

        /// <summary>
        /// Nom complet de l'utilisateur (Prénom + Nom)
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'utilisateur
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Nom de famille de l'utilisateur
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Adresse email de l'utilisateur
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Numéro de téléphone de l'utilisateur
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Nom de l'équipe de l'utilisateur
        /// </summary>
        public string? TeamName { get; set; }

        /// <summary>
        /// Liste des rôles attribués à l'utilisateur
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// Indique si l'utilisateur est actif
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Nombre de tâches actuellement assignées à cet utilisateur
        /// </summary>
        public int CurrentTaskCount { get; set; } = 0;

        /// <summary>
        /// Nombre de tâches en cours pour cet utilisateur
        /// </summary>
        public int ActiveTaskCount { get; set; } = 0;
    }
}
