using Dior.Library.BO;
using Dior.Library.DAO;

namespace Dior.Service.Services
{
    /// <summary>
    /// Service de gestion des tâches (wrapping DAO)
    /// </summary>
    public class TaskService
    {
        private readonly ITaskDao _taskDao;

        /// <summary>Constructor</summary>
        public TaskService(ITaskDao taskDao)
        {
            _taskDao = taskDao;
        }

        /// <summary>Retourne toutes les tâches</summary>
        public List<TaskBO> GetAllTasks()
        {
            return _taskDao.GetAll();
        }

        /// <summary>Retourne une tâche par id</summary>
        public TaskBO? GetTaskById(long id)
        {
            return _taskDao.GetById(id);
        }

        /// <summary>Crée une tâche</summary>
        public void CreateTask(TaskBO task)
        {
            _taskDao.Create(task);
        }

        /// <summary>Met à jour une tâche</summary>
        public void UpdateTask(TaskBO task)
        {
            _taskDao.Update(task);
        }

        /// <summary>Supprime une tâche</summary>
        public void DeleteTask(long id)
        {
            _taskDao.Delete(id);
        }

        /// <summary>
        /// Récupère les tâches assignées à un utilisateur
        /// </summary>
        public List<TaskBO> GetTasksAssignedToUser(long userId)
        {
            return _taskDao.GetTasksAssignedToUser(userId);
        }

        /// <summary>
        /// Récupère les tâches créées par un utilisateur
        /// </summary>
        public List<TaskBO> GetTasksCreatedByUser(long userId)
        {
            return _taskDao.GetTasksCreatedByUser(userId);
        }

        /// <summary>
        /// Récupère les tâches par statut
        /// </summary>
        public List<TaskBO> GetTasksByStatus(string status)
        {
            return _taskDao.GetTasksByStatus(status);
        }

        /// <summary>
        /// Compte les tâches par statut pour un utilisateur
        /// </summary>
        public int GetTaskCountByStatusForUser(long userId, string status)
        {
            return _taskDao.GetTaskCountByStatusForUser(userId, status);
        }

        /// <summary>Met à jour le statut d'une tâche</summary>
        public void UpdateTaskStatus(long taskId, string newStatus, string lastEditBy)
        {
            _taskDao.UpdateTaskStatus(taskId, newStatus, lastEditBy);
        }

        /// <summary>Retourne les tâches actives d'un utilisateur</summary>
        public List<TaskBO> GetActiveTasksForUser(long userId)
        {
            return _taskDao.GetTasksAssignedToUser(userId)
                .Where(t => t.Status == "En attente" || t.Status == "En cours")
                .ToList();
        }

        /// <summary>Réassigne une tâche</summary>
        public void ReassignTask(long taskId, long newAssignedToUserId, string reassignedBy)
        {
            var task = _taskDao.GetById(taskId);
            if (task != null)
            {
                task.AssignedToUserId = newAssignedToUserId;
                task.LastEditAt = DateTime.Now;
                task.LastEditBy = reassignedBy;
                _taskDao.Update(task);
            }
        }
    }
}