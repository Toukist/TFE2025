using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Database.Entities
{
    public class UserAccessCompetency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public int AccessCompetencyId { get; set; }
        
        [ForeignKey(nameof(AccessCompetencyId))]
        public virtual AccessCompetency AccessCompetency { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }
    }
}