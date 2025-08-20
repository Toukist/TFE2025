using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("ACCESS")]
    public class Access
    {
        public Access()
        {
            UserAccesses = new HashSet<UserAccess>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? BadgeNumber { get; set; }

        [MaxLength(50)]
        public string? BadgePhysicalNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string? LastEditBy { get; set; }

        // Navigation properties
        public virtual ICollection<UserAccess> UserAccesses { get; set; }
    }
}
