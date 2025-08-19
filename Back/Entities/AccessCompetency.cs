using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Database.Entities
{
    public class AccessCompetency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        
        [ForeignKey(nameof(ParentId))]
        public virtual AccessCompetency Parent { get; set; }
        
        public virtual ICollection<AccessCompetency> Children { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string LastEditBy { get; set; }

        // Navigation properties
        public virtual ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; }
    }
}