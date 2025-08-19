using Dior.Library.DTO;
using Dior.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace Dior.Service.Host.Controllers
{
    /// <summary>
    /// Contrôleur de messagerie interne pour communication d'équipe
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("?? Messagerie - Communication interne entre utilisateurs et équipes")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        /// <summary>
        /// Envoyer un message à toute l'équipe (Manager uniquement)
        /// </summary>
        /// <param name="request">Contenu du message d'équipe</param>
        /// <returns>Message créé</returns>
        /// <remarks>
        /// Permet aux **Managers** d'envoyer des messages à tous les membres de leur équipe.
        /// 
        /// **Exemple d'utilisation :**
        /// - Annonces importantes
        /// - Réunions d'équipe
        /// - Instructions de travail
        /// - Notifications urgentes
        /// 
        /// Le message sera automatiquement envoyé à tous les membres de l'équipe spécifiée.
        /// </remarks>
        [HttpPost("team")]
        [Authorize(Roles = "Manager")]
        [SwaggerOperation(
            Summary = "?? Message à l'équipe",
            Description = "Envoie un message à tous les membres d'une équipe (Manager uniquement)"
        )]
        [SwaggerResponse(200, "Message envoyé avec succès", typeof(MessageDto))]
        [SwaggerResponse(400, "Données invalides")]
        [SwaggerResponse(403, "Accès refusé - Rôle Manager requis")]
        public async Task<ActionResult> SendToTeam([FromBody] CreateMessageRequest request)
        {
            var managerId = GetCurrentUserId();
            
            if (!request.RecipientTeamId.HasValue)
                return BadRequest("TeamId requis pour message d'équipe");
            
            var message = await _messageService.SendToTeamAsync(managerId, request);
            
            return Ok(message);
        }
        
        /// <summary>
        /// Envoyer un message individuel à un utilisateur
        /// </summary>
        /// <param name="request">Contenu du message individuel</param>
        /// <returns>Message créé</returns>
        /// <remarks>
        /// Permet d'envoyer un message privé à un utilisateur spécifique.
        /// 
        /// **Types de messages supportés :**
        /// - `General` : Message standard
        /// - `Urgent` : Message prioritaire
        /// - `Task` : Attribution de tâche
        /// - `Information` : Information simple
        /// </remarks>
        [HttpPost("user")]
        [SwaggerOperation(
            Summary = "?? Message individuel",
            Description = "Envoie un message privé à un utilisateur spécifique"
        )]
        [SwaggerResponse(200, "Message envoyé", typeof(MessageDto))]
        [SwaggerResponse(400, "Utilisateur destinataire manquant")]
        public async Task<ActionResult> SendToUser([FromBody] CreateMessageRequest request)
        {
            if (!request.RecipientUserId.HasValue)
                return BadRequest("Destinataire requis");
            
            var senderId = GetCurrentUserId();
            var message = await _messageService.SendToUserAsync(senderId, request);
            
            return Ok(message);
        }
        
        /// <summary>
        /// Récupérer tous mes messages reçus
        /// </summary>
        /// <returns>Liste des messages reçus</returns>
        [HttpGet("my")]
        [SwaggerOperation(
            Summary = "?? Mes messages",
            Description = "Récupère tous les messages reçus (équipe + individuels)"
        )]
        [SwaggerResponse(200, "Liste des messages", typeof(List<MessageDto>))]
        public async Task<ActionResult<List<MessageDto>>> GetMyMessages()
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetUserMessagesAsync(userId);
            return Ok(messages);
        }
        
        /// <summary>
        /// Marquer un message comme lu
        /// </summary>
        /// <param name="id">ID du message</param>
        /// <returns>Statut de l'opération</returns>
        [HttpPatch("{id}/read")]
        [SwaggerOperation(
            Summary = "? Marquer comme lu",
            Description = "Marque un message comme lu et met à jour la date de lecture"
        )]
        [SwaggerResponse(204, "Message marqué comme lu")]
        [SwaggerResponse(404, "Message non trouvé")]
        public async Task<ActionResult> MarkAsRead(long id)
        {
            await _messageService.MarkAsReadAsync(id);
            return NoContent();
        }
        
        /// <summary>
        /// Obtenir le nombre de messages non lus
        /// </summary>
        /// <returns>Nombre de messages non lus</returns>
        [HttpGet("unread-count")]
        [SwaggerOperation(
            Summary = "?? Messages non lus",
            Description = "Retourne le nombre de messages non lus pour l'utilisateur connecté"
        )]
        [SwaggerResponse(200, "Nombre de messages non lus", typeof(int))]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var count = await _messageService.GetUnreadCountAsync(userId);
            return Ok(count);
        }
        
        private long GetCurrentUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
    }
}