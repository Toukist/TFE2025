using System.ComponentModel.DataAnnotations;

namespace Dior.Library.DTO
{
    public class ProjetDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Le nom du projet est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Nom { get; set; } = string.Empty; // ATTENTION: "Nom" pas "Name"
        
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
        
        // Dates
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        
        // Relations avec NOMS
        [Required(ErrorMessage = "L'équipe est obligatoire")]
        public int TeamId { get; set; }
        
        public string TeamName { get; set; } = string.Empty; // Propriété calculée
        
        public long ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        
        // Type de projet
        public string Type { get; set; } = "Equipe"; // "Equipe" ou "Personnel"
        
        // Audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        
        // Statut calculé
        public string Statut 
        { 
            get 
            {
                if (!DateDebut.HasValue) return "Planifié";
                if (DateDebut.Value > DateTime.Now) return "Planifié";
                if (DateFin.HasValue && DateFin.Value < DateTime.Now) return "Terminé";
                return "En cours";
            }
        }
        
        // Progression (pourcentage)
        public int Progress { get; set; } = 0;
    }

    public class CreateProjetRequest
    {
        [Required(ErrorMessage = "Le nom du projet est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Nom { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
        
        public int? TeamId { get; set; }
        public long? ManagerId { get; set; }
        
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        
        public string Type { get; set; } = "Equipe";
    }

    public class UpdateProjetRequest
    {
        [Required(ErrorMessage = "Le nom du projet est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Nom { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
        
        public int? TeamId { get; set; }
        public long? ManagerId { get; set; }
        
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        
        public string Type { get; set; } = "Equipe";
        public int Progress { get; set; } = 0;
    }
}