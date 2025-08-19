using Dior.Library.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AccessCompetency
{
    public AccessCompetency()
    {
        Children = new List<AccessCompetency>();
        UserAccessCompetencies = new List<UserAccessCompetency>();
    }

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public int? ParentId { get; set; }
    [ForeignKey(nameof(ParentId))]
    public AccessCompetency Parent { get; set; } = null!;
    public ICollection<AccessCompetency> Children { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; }

    public DateTime? LastEditAt { get; set; }
    [MaxLength(100)]
    public string LastEditBy { get; set; }

    // Navigation
    public ICollection<UserAccessCompetency> UserAccessCompetencies { get; set; } = null!;
    //  public string Description { get; set; }
}