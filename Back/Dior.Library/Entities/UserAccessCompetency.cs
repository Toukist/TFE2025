using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dior.Library.Entities
{
    [Table("USER_ACCESS_COMPETENCY")]
    public class UserAccessCompetency
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int AccessCompetencyId { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? LastEditAt { get; set; }

        [MaxLength(100)]
        public string? LastEditBy { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual AccessCompetency? AccessCompetency { get; set; }
    }
}}