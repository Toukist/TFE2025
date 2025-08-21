namespace Dior.Database.DTOs.User
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsActive { get; set; }
        public string LastEditBy { get; set; }
    }
}