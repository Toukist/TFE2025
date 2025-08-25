using System;
using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO.Message
{
    /// <summary>
    /// DTO pour les messages
    /// </summary>
    public class MessageDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Individual, Team, Broadcast
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public int? RecipientId { get; set; }
        public string? RecipientName { get; set; }
        public int? RecipientTeamId { get; set; }
        public string? RecipientTeamName { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    /// <summary>
    /// DTO pour créer un message
    /// </summary>
    public class CreateMessageDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = "Individual";
        
        public int? RecipientId { get; set; }
        public int? RecipientTeamId { get; set; }
    }

    /// <summary>
    /// Classe pour les requêtes de message (compatibility)
    /// </summary>
    public class CreateMessageRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "Individual";
        public int? RecipientId { get; set; }
        public int? RecipientTeamId { get; set; }
    }
}