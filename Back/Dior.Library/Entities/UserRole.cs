using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserRole
{


    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    [Required]
    public int RoleDefinitionId { get; set; }
    [ForeignKey(nameof(RoleDefinitionId))]
    public RoleDefinition RoleDefinition { get; set; }

    public DateTime? ExpiresAt { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; }
    public DateTime? LastEditAt { get; set; }
    [MaxLength(100)]
    public string LastEditBy { get; set; }
}