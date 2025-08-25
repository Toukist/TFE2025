using Dior.Library.BO;

namespace Dior.Library.DAO
{
    public interface ITaskDao
    {
        List<TaskBO> GetAll();
        TaskBO? GetById(long id);
        void Create(TaskBO task);
        void Update(TaskBO task);
        void Delete(long id);

        List<TaskBO> GetTasksAssignedToUser(long userId);
        List<TaskBO> GetTasksCreatedByUser(long userId);
        List<TaskBO> GetTasksByStatus(string status);
        int GetTaskCountByStatusForUser(long userId, string status);
        void UpdateTaskStatus(long taskId, string newStatus, string lastEditBy);
    }
}