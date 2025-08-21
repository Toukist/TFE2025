namespace Dior.Database.DTOs.AccessCompetency
{
    public class UpdateAccessCompetencyDto
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string LastEditBy { get; set; }
    }
}