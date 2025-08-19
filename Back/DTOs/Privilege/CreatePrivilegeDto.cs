namespace Dior.Database.DTOs.Privilege
{
    public class CreatePrivilegeDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}