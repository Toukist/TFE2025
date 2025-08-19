namespace Dior.Database.DTOs.UserAccessCompetency
{
    public class CreateUserAccessCompetencyDto
    {
        public int UserId { get; set; }
        public int AccessCompetencyId { get; set; }
        public string CreatedBy { get; set; }
    }
}