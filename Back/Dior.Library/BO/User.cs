namespace Dior.Library.BO
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsActive { get; set; }
        public string LastEditBy { get; set; }
        public DateTime LastEditAt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? TeamId { get; set; } // Ajout TeamId
        public string TeamName { get; set; } // Ajout TeamName (lecture seule)
    }
}