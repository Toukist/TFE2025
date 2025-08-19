using Dior.Library.BO;
using Dior.Library.DAO;

namespace Dior.Service.Services
{
    public class TaskService
    {
        private readonly ITaskDao _taskDao;

        public TaskService(ITaskDao taskDao)
        {
            _taskDao = taskDao;
        }

        public List<TaskBO> GetAllTasks()
        {
            return _taskDao.GetAll();
        }

        public TaskBO? GetTaskById(int id)
        {
            return _taskDao.GetById(id);
        }

        public void CreateTask(TaskBO task)
        {
            _taskDao.Create(task);
        }

        public void UpdateTask(TaskBO task)
        {
            _taskDao.Update(task);
        }

        public void DeleteTask(int id)
        {
            _taskDao.Delete(id);
        }
        // AJOUTER À LA CLASSE TaskService EXISTANTE

        /// <summary>
        /// Récupérer les tâches assignées à un utilisateur (pour opérateurs)
        /// </summary>
        public List<TaskBO> GetTasksAssignedToUser(int userId)
        {
            return _taskDao.GetTasksAssignedToUser(userId);
        }

        /// <summary>
        /// Récupérer les tâches créées par un utilisateur (pour managers)
        /// </summary>
        public List<TaskBO> GetTasksCreatedByUser(int userId)
        {
            return _taskDao.GetTasksCreatedByUser(userId);
        }

        /// <summary>
        /// Récupérer les tâches par statut
        /// </summary>
        public List<TaskBO> GetTasksByStatus(string status)
        {
            return _taskDao.GetTasksByStatus(status);
        }

        /// <summary>
        /// Compter les tâches par statut pour un utilisateur
        /// </summary>
        public int GetTaskCountByStatusForUser(int userId, string status)
        {
            return _taskDao.GetTaskCountByStatusForUser(userId, status);
        }


        public void UpdateTaskStatus(int taskId, string newStatus, string lastEditBy)
        {
            _taskDao.UpdateTaskStatus(taskId, newStatus, lastEditBy);
        }


        public List<TaskBO> GetActiveTasksForUser(int userId)
        {
            return _taskDao.GetTasksAssignedToUser(userId)
                .Where(t => t.Status == "En attente" || t.Status == "En cours")
                .ToList();
        }


        public void ReassignTask(int taskId, int newAssignedToUserId, string reassignedBy)
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