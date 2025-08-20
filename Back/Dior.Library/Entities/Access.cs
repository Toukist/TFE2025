using System.ComponentModel.DataAnnotations;
using Dior.Library.Entities;

namespace Dior.Library.Entities
{
    public class Access
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string BadgePhysicalNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        // Navigation property referencing the assignments of this access badge to users
        public ICollection<UserAccess> UserAccesses { get; set; }
    }
}
