public class UserAccessCompetencyDto
{
    public int Id { get; set; }
    public long UserId { get; set; } // ✅ Changé de int à long
    public int AccessCompetencyId { get; set; }

    public string Name { get; set; }           // ajouté
    public bool IsActive { get; set; }         // ajouté

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastEditAt { get; set; }
    public string LastEditBy { get; set; }
}
