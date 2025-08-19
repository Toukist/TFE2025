using Dior.Library.DTO;
using Dior.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace Dior.Service.Host.Controllers
{
    /// <summary>
    /// Contr�leur de messagerie interne pour communication d'�quipe
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [SwaggerTag("?? Messagerie - Communication interne entre utilisateurs et �quipes")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        /// <summary>
        /// Envoyer un message � toute l'�quipe (Manager uniquement)
        /// </summary>
        /// <param name="request">Contenu du message d'�quipe</param>
        /// <returns>Message cr��</returns>
        /// <remarks>
        /// Permet aux **Managers** d'envoyer des messages � tous les membres de leur �quipe.
        /// 
        /// **Exemple d'utilisation :**
        /// - Annonces importantes
        /// - R�unions d'�quipe
        /// - Instructions de travail
        /// - Notifications urgentes
        /// 
        /// Le message sera automatiquement envoy� � tous les membres de l'�quipe sp�cifi�e.
        /// </remarks>
        [HttpPost("team")]
        [Authorize(Roles = "Manager")]
        [SwaggerOperation(
            Summary = "?? Message � l'�quipe",
            Description = "Envoie un message � tous les membres d'une �quipe (Manager uniquement)"
        )]
        [SwaggerResponse(200, "Message envoy� avec succ�s", typeof(MessageDto))]
        [SwaggerResponse(400, "Donn�es invalides")]
        [SwaggerResponse(403, "Acc�s refus� - R�le Manager requis")]
        public async Task<ActionResult> SendToTeam([FromBody] CreateMessageRequest request)
        {
            var managerId = GetCurrentUserId();
            
            if (!request.RecipientTeamId.HasValue)
                return BadRequest("TeamId requis pour message d'�quipe");
            
            var message = await _messageService.SendToTeamAsync(managerId, request);
            
            return Ok(message);
        }
        
        /// <summary>
        /// Envoyer un message individuel � un utilisateur
        /// </summary>
        /// <param name="request">Contenu du message individuel</param>
        /// <returns>Message cr��</returns>
        /// <remarks>
        /// Permet d'envoyer un message priv� � un utilisateur sp�cifique.
        /// 
        /// **Types de messages support�s :**
        /// - `General` : Message standard
        /// - `Urgent` : Message prioritaire
        /// - `Task` : Attribution de t�che
        /// - `Information` : Information simple
        /// </remarks>
        [HttpPost("user")]
        [SwaggerOperation(
            Summary = "?? Message individuel",
            Description = "Envoie un message priv� � un utilisateur sp�cifique"
        )]
        [SwaggerResponse(200, "Message envoy�", typeof(MessageDto))]
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
        /// R�cup�rer tous mes messages re�us
        /// </summary>
        /// <returns>Liste des messages re�us</returns>
        [HttpGet("my")]
        [SwaggerOperation(
            Summary = "?? Mes messages",
            Description = "R�cup�re tous les messages re�us (�quipe + individuels)"
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
        /// <returns>Statut de l'op�ration</returns>
        [HttpPatch("{id}/read")]
        [SwaggerOperation(
            Summary = "? Marquer comme lu",
            Description = "Marque un message comme lu et met � jour la date de lecture"
        )]
        [SwaggerResponse(204, "Message marqu� comme lu")]
        [SwaggerResponse(404, "Message non trouv�")]
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
            Description = "Retourne le nombre de messages non lus pour l'utilisateur connect�"
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