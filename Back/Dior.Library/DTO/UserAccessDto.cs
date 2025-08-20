namespace Dior.Library.DTO
{
    public class UserAccessDto
    {
        public int Id { get; set; }
        public long UserId { get; set; } // ✅ Changé de int à long
        public int AccessId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }

}
