namespace Dior.Database.DTOs.Access
{
    public class CreateAccessDto
    {
        public string BadgePhysicalNumber { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}