using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class MessageDto
    {
        public long Id { get; set; }
        
        // Expéditeur
        public long SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        
        // Destinataire(s)
        public long? RecipientUserId { get; set; } // Si message individuel
        public string? RecipientUserName { get; set; }
        
        public int? RecipientTeamId { get; set; } // Si message d'équipe
        public string? RecipientTeamName { get; set; }
        
        // Contenu
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
        
        // Statut
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        
        // Timestamps
        public DateTime SentAt { get; set; } = DateTime.Now;
        
        // Type
        public string MessageType { get; set; } = "General"; // General, Task, Alert, Announcement
    }
    
    public class CreateMessageRequest
    {
        [Required]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public long? RecipientUserId { get; set; }
        public int? RecipientTeamId { get; set; }
        
        public string Priority { get; set; } = "Normal";
        public string MessageType { get; set; } = "General";
    }
}