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
        private readonly TaskService _taskService; // Service existant conserv� pour compatibilit�

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// GET api/Task - R�cup�re toutes les t�ches
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetAll()
        {
            var tasks = await Task.Run(() => _taskService.GetAllTasks());
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// GET api/Task/{id} - R�cup�re une t�che par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetById(long id)
        {
            var task = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (task == null)
                return NotFound($"T�che avec l'ID {id} non trouv�e");

            return Ok(MapToDto(task));
        }

        /// <summary>
        /// GET api/Task/user/{userId} - R�cup�re les t�ches assign�es � un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<TaskDto>>> GetByUserId(long userId)
        {
            var tasks = await Task.Run(() => _taskService.GetTasksAssignedToUser((int)userId));
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// GET api/Task/status/{status} - R�cup�re les t�ches par statut
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<TaskDto>>> GetByStatus(string status)
        {
            var tasks = await Task.Run(() => _taskService.GetTasksByStatus(status));
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        /// <summary>
        /// POST api/Task - Cr�e une nouvelle t�che
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
                AssignedToUserId = (int)request.AssignedToUserId, // Conversion pour compatibilit� avec BO existant
                CreatedByUserId = (int)request.CreatedByUserId,
                CreatedAt = DateTime.Now,
                CreatedBy = userName,
                DueDate = request.DueDate
            };

            await Task.Run(() => _taskService.CreateTask(taskBo));

            return CreatedAtAction(nameof(GetById), new { id = taskBo.Id }, MapToDto(taskBo));
        }

        /// <summary>
        /// PUT api/Task/{id} - Met � jour une t�che compl�tement
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"T�che avec l'ID {id} non trouv�e");

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
        /// PATCH api/Task/{id}/status - Met � jour uniquement le statut d'une t�che
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(long id, [FromBody] UpdateTaskStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"T�che avec l'ID {id} non trouv�e");

            await Task.Run(() => _taskService.UpdateTaskStatus((int)id, request.Status, userName));
            return NoContent();
        }

        /// <summary>
        /// PATCH api/Task/{id}/assign - Assigne une t�che � un autre utilisateur
        /// </summary>
        [HttpPatch("{id}/assign")]
        public async Task<ActionResult> AssignToUser(long id, [FromBody] AssignTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"T�che avec l'ID {id} non trouv�e");

            await Task.Run(() => _taskService.ReassignTask((int)id, (int)request.AssignedToUserId, userName));
            return NoContent();
        }

        /// <summary>
        /// DELETE api/Task/{id} - Supprime une t�che
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var existingTask = await Task.Run(() => _taskService.GetTaskById((int)id));
            if (existingTask == null)
                return NotFound($"T�che avec l'ID {id} non trouv�e");

            await Task.Run(() => _taskService.DeleteTask((int)id));
            return NoContent();
        }

        // ===== M�THODES DE MAPPING =====

        private TaskDto MapToDto(TaskBO bo)
        {
            return new TaskDto
            {
                Id = bo.Id,
                Title = bo.Title,
                Description = bo.Description,
                Status = bo.Status,
                AssignedToUserId = bo.AssignedToUserId,
                // TODO: R�cup�rer les noms d'utilisateurs depuis UserService if n�cessaire
                AssignedToUserName = null, // Propri�t� calcul�e � impl�menter
                CreatedByUserId = bo.CreatedByUserId,
                CreatedByUserName = null, // Propri�t� calcul�e � impl�menter
                CreatedAt = bo.CreatedAt,
                DueDate = bo.DueDate,
                CreatedBy = bo.CreatedBy,
                LastEditAt = bo.LastEditAt,
                LastEditBy = bo.LastEditBy
            };
        }
    }
}