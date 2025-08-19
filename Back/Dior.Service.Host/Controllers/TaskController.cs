using Dior.Library.BO;
using Dior.Library.DTO;
using Dior.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authentification requise
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService; // Service existant conservé pour compatibilité

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// GET api/Task - Récupère toutes les tâches
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll()
        {
            var tasks = await Task.Run(() => _taskService.GetAllTasks());
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// GET api/Task/{id} - Récupère une tâche par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetById(long id)
        {
            var task = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (task == null)
                return NotFound($"Tâche avec l'ID {id} non trouvée");

            return Ok(MapToDto(task));
        }

        /// <summary>
        /// GET api/Task/user/{userId} - Récupère les tâches assignées à un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<TaskDto>>> GetByUserId(long userId)
        {
            var tasks = await Task.Run(() => _taskService.GetTasksAssignedToUser((int)userId));
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// GET api/Task/status/{status} - Récupère les tâches par statut
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<TaskDto>>> GetByStatus(string status)
        {
            var tasks = await Task.Run(() => _taskService.GetTasksByStatus(status));
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// POST api/Task - Crée une nouvelle tâche
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var taskBo = new TaskBO
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                AssignedToUserId = (int)request.AssignedToUserId, // Conversion pour compatibilité avec BO existant
                CreatedByUserId = (int)request.CreatedByUserId,
                CreatedAt = DateTime.Now,
                CreatedBy = userName,
                DueDate = request.DueDate
            };

            await Task.Run(() => _taskService.CreateTask(taskBo));

            return CreatedAtAction(nameof(GetById), new { id = taskBo.Id }, MapToDto(taskBo));
        }

        /// <summary>
        /// PUT api/Task/{id} - Met à jour une tâche complètement
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"Tâche avec l'ID {id} non trouvée");

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var taskBo = new TaskBO
            {
                Id = (int)id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                AssignedToUserId = (int)request.AssignedToUserId,
                CreatedByUserId = existingTask.CreatedByUserId, // Conserver l'original
                CreatedAt = existingTask.CreatedAt, // Conserver l'original
                CreatedBy = existingTask.CreatedBy, // Conserver l'original
                LastEditAt = DateTime.Now,
                LastEditBy = userName,
                DueDate = request.DueDate
            };

            await Task.Run(() => _taskService.UpdateTask(taskBo));
            return NoContent();
        }

        /// <summary>
        /// PATCH api/Task/{id}/status - Met à jour uniquement le statut d'une tâche
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(long id, [FromBody] UpdateTaskStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"Tâche avec l'ID {id} non trouvée");

            await Task.Run(() => _taskService.UpdateTaskStatus((int)id, request.Status, userName));
            return NoContent();
        }

        /// <summary>
        /// PATCH api/Task/{id}/assign - Assigne une tâche à un autre utilisateur
        /// </summary>
        [HttpPatch("{id}/assign")]
        public async Task<ActionResult> AssignToUser(long id, [FromBody] AssignTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"Tâche avec l'ID {id} non trouvée");

            await Task.Run(() => _taskService.ReassignTask((int)id, (int)request.AssignedToUserId, userName));
            return NoContent();
        }

        /// <summary>
        /// DELETE api/Task/{id} - Supprime une tâche
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"Tâche avec l'ID {id} non trouvée");

            await Task.Run(() => _taskService.DeleteTask((int)id));
            return NoContent();
        }

        // ===== MÉTHODES DE MAPPING =====

        private TaskDto MapToDto(TaskBO bo)
        {
            return new TaskDto
            {
                Id = bo.Id,
                Title = bo.Title,
                Description = bo.Description,
                Status = bo.Status,
                AssignedToUserId = bo.AssignedToUserId,
                // TODO: Récupérer les noms d'utilisateurs depuis UserService if nécessaire
                AssignedToUserName = null, // Propriété calculée à implémenter
                CreatedByUserId = bo.CreatedByUserId,
                CreatedByUserName = null, // Propriété calculée à implémenter
                CreatedAt = bo.CreatedAt,
                DueDate = bo.DueDate,
                CreatedBy = bo.CreatedBy,
                LastEditAt = bo.LastEditAt,
                LastEditBy = bo.LastEditBy
            };
        }
    }
}