using Dior.Library.DTO;
using Dior.Library.Interfaces.UserInterface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dior.Service.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Authentification requise pour toutes les actions
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        
        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET api/Notification/user/{userId} - Récupère toutes les notifications d'un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<NotificationDto>>> GetByUserId(long userId)
        {
            // Vérifier si l'utilisateur peut accéder à ces notifications
            var currentUserIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("admin");
            
            if (!isAdmin && (!long.TryParse(currentUserIdClaim, out var currentUserId) || currentUserId != userId))
            {
                return Forbid("Vous ne pouvez consulter que vos propres notifications");
            }

            var notifications = await _service.GetByUserIdAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// GET api/Notification/user/{userId}/unread - Récupère les notifications non lues d'un utilisateur
        /// </summary>
        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<List<NotificationDto>>> GetUnreadByUserId(long userId)
        {
            // Vérifier si l'utilisateur peut accéder à ces notifications
            var currentUserIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("admin");
            
            if (!isAdmin && (!long.TryParse(currentUserIdClaim, out var currentUserId) || currentUserId != userId))
            {
                return Forbid("Vous ne pouvez consulter que vos propres notifications");
            }

            var notifications = await _service.GetUnreadByUserIdAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// PATCH api/Notification/{id}/read - Marque une notification comme lue
        /// </summary>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _service.MarkAsReadAsync(id);
            return result ? NoContent() : NotFound($"Notification avec l'ID {id} non trouvée");
        }

        /// <summary>
        /// PATCH api/Notification/user/{userId}/read-all - Marque toutes les notifications d'un utilisateur comme lues
        /// </summary>
        [HttpPatch("user/{userId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead(long userId)
        {
            // Vérifier si l'utilisateur peut modifier ces notifications
            var currentUserIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("admin");
            
            if (!isAdmin && (!long.TryParse(currentUserIdClaim, out var currentUserId) || currentUserId != userId))
            {
                return Forbid("Vous ne pouvez modifier que vos propres notifications");
            }

            var result = await _service.MarkAllAsReadAsync(userId);
            return result ? NoContent() : BadRequest("Erreur lors de la mise à jour des notifications");
        }

        /// <summary>
        /// POST api/Notification - Crée une nouvelle notification
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,manager")] // Seuls les admins et managers peuvent créer des notifications
        public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var notification = await _service.CreateAsync(request, userName);
            
            return CreatedAtAction(nameof(GetByUserId), new { userId = notification.UserId }, notification);
        }

        /// <summary>
        /// POST api/Notification/bulk - Envoie des notifications en lot à plusieurs utilisateurs
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Roles = "admin,manager")] // Seuls les admins et managers peuvent envoyer des notifications en lot
        public async Task<IActionResult> SendBulkNotifications([FromBody] BulkNotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirst("username")?.Value ?? User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
            var result = await _service.SendBulkNotificationsAsync(request.UserIds, request.Message, request.Type, userName);
            
            return result ? 
                Ok(new { message = $"Notifications envoyées à {request.UserIds.Count} utilisateurs", success = true }) :
                BadRequest(new { message = "Erreur lors de l'envoi des notifications", success = false });
        }

        /// <summary>
        /// DELETE api/Notification/{id} - Supprime une notification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound($"Notification avec l'ID {id} non trouvée");
        }

        // ===== MÉTHODES LEGACY (conservées pour compatibilité) =====

        /// <summary>
        /// POST /api/Notification/mark-read - Marque plusieurs notifications comme lues (Legacy)
        /// </summary>
        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkRead([FromBody] MarkReadRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("admin");
            
            if (string.IsNullOrWhiteSpace(userIdClaim) || (!int.TryParse(userIdClaim, out var userId) && !isAdmin))
                return Unauthorized();

            // Marquer les notifications comme lues
            foreach (var notificationId in request.Ids)
            {
                await _service.MarkAsReadAsync(notificationId);
            }

            return NoContent();
        }

        /// <summary>
        /// Récupère toutes les notifications d'un utilisateur (Legacy - support int)
        /// </summary>
        [HttpGet]
        public ActionResult<List<NotificationDto>> GetByUserId([FromQuery] int userId)
        {
            return Ok(_service.GetByUserId(userId));
        }
    }

    public class MarkReadRequest
    {
        public List<int> Ids { get; set; } = new();
    }
}