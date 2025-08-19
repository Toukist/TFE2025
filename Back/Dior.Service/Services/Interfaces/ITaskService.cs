using Dior.Library.DTO;

namespace Dior.Service.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAllAsync();
        Task<List<TaskDto>> GetByUserIdAsync(long userId);
        Task<List<TaskDto>> GetByStatusAsync(string status);
        Task<TaskDto?> GetByIdAsync(long id);
        Task<TaskDto> CreateAsync(CreateTaskRequest request, string createdBy);
        Task<bool> UpdateAsync(long id, UpdateTaskRequest request, string lastEditBy);
        Task<bool> UpdateStatusAsync(long id, string status, string lastEditBy);
        Task<bool> AssignToUserAsync(long taskId, long userId, string assignedBy);
        Task<bool> DeleteAsync(long id);
    }
}