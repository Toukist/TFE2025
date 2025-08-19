namespace Dior.Library.DTO
{
    /// <summary>
    /// DTO contenant un résumé statistique des tâches pour un utilisateur
    /// </summary>
    public class TaskSummaryDto
    {
        /// <summary>
        /// ID de l'utilisateur concerné par ce résumé
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nom complet de l'utilisateur
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Nombre total de tâches assignées à l'utilisateur
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Nombre de tâches en attente
        /// </summary>
        public int PendingTasks { get; set; }

        /// <summary>
        /// Nombre de tâches en cours d'exécution
        /// </summary>
        public int InProgressTasks { get; set; }

        /// <summary>
        /// Nombre de tâches terminées
        /// </summary>
        public int CompletedTasks { get; set; }

        /// <summary>
        /// Nombre de tâches bloquées
        /// </summary>
        public int BlockedTasks { get; set; }

        /// <summary>
        /// Pourcentage de tâches terminées (calculé)
        /// </summary>
        public decimal CompletionRate => TotalTasks > 0 ? (decimal)CompletedTasks / TotalTasks * 100 : 0;

        /// <summary>
        /// Nombre de tâches en retard (avec échéance dépassée)
        /// </summary>
        public int OverdueTasks { get; set; }

        /// <summary>
        /// Date de dernière mise à jour de ce résumé
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
