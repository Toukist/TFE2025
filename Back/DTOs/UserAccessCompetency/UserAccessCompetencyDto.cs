using System;

namespace Dior.Database.DTOs.UserAccessCompetency
{
    public class UserAccessCompetencyDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AccessCompetencyId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}