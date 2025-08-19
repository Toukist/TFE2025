using Dior.Library.BO;

namespace Dior.Library.DAO
{
    public interface ITaskDao
    {
        List<TaskBO> GetAll();
        TaskBO? GetById(int id);
        void Create(TaskBO task);
        void Update(TaskBO task);
        void Delete(int id);

        List<TaskBO> GetTasksAssignedToUser(int userId);
        List<TaskBO> GetTasksCreatedByUser(int userId);
        List<TaskBO> GetTasksByStatus(string status);
        int GetTaskCountByStatusForUser(int userId, string status);
        void UpdateTaskStatus(int taskId, string newStatus, string lastEditBy);
    }
}