namespace Dior.Database.DTOs.UserAccess
{
    public class CreateUserAccessDto
    {
        public int UserId { get; set; }
        public int AccessId { get; set; }
        public string CreatedBy { get; set; }
    }
}