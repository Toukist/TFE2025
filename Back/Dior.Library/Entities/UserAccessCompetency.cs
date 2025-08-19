using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ACCESS_COMPETENCY")]
    public class UserAccessCompetency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public int AccessCompetencyId { get; set; }
        [ForeignKey(nameof(AccessCompetencyId))]
        public AccessCompetency AccessCompetency { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        [MaxLength(100)]
        public string LastEditBy { get; set; }
    }
}