using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dior.Database.Entities
{
    public class Access
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string BadgePhysicalNumber { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<UserAccess> UserAccesses { get; set; }
    }
}