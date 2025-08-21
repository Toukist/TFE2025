namespace Dior.Database.DTOs.AccessCompetency
{
    public class CreateAccessCompetencyDto
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}