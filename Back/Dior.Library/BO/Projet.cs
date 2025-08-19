namespace Dior.Library.BO
{
    public class Projet
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty; // ATTENTION: "Nom" pas "Name"
        public string? Description { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int TeamId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }
}