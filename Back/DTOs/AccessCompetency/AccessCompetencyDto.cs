using System;

namespace Dior.Database.DTOs.AccessCompetency
{
    public class AccessCompetencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }   
        public string LastEditBy { get; set; }
    }
}