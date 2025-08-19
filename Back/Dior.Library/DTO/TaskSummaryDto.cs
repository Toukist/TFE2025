namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO contenant un r�sum� statistique des t�ches pour un utilisateur
    /// </summary>
    public class TaskSummaryDto
    {
        /// <summary>
        /// ID de l'utilisateur concern� par ce r�sum�
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nom complet de l'utilisateur
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Nombre total de t�ches assign�es � l'utilisateur
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Nombre de t�ches en attente
        /// </summary>
        public int PendingTasks { get; set; }

        /// <summary>
        /// Nombre de t�ches en cours d'ex�cution
        /// </summary>
        public int InProgressTasks { get; set; }

        /// <summary>
        /// Nombre de t�ches termin�es
        /// </summary>
        public int CompletedTasks { get; set; }

        /// <summary>
        /// Nombre de t�ches bloqu�es
        /// </summary>
        public int BlockedTasks { get; set; }

        /// <summary>
        /// Pourcentage de t�ches termin�es (calcul�)
        /// </summary>
        public decimal CompletionRate => TotalTasks > 0 ? (decimal)CompletedTasks / TotalTasks * 100 : 0;

        /// <summary>
        /// Nombre de t�ches en retard (avec �ch�ance d�pass�e)
        /// </summary>
        public int OverdueTasks { get; set; }

        /// <summary>
        /// Date de derni�re mise � jour de ce r�sum�
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
