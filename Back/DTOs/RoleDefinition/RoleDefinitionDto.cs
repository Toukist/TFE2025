namespace Dior.Library.DTO
{
    public class RoleDefinitionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentRoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}
using Dior.Library.DTO;
