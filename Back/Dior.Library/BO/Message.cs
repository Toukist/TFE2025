namespace Dior.Library.BO
{
    public class Message
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long? RecipientUserId { get; set; }
        public int? RecipientTeamId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal";
        public string MessageType { get; set; } = "General";
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}