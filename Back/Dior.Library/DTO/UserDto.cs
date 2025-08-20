#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Dior.Library.Interfaces.UserInterface.Services;

namespace Dior.Library.DTO
{
    public class UserDto
    {
        // Identité
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        // Contact
        public string? Email { get; set; }
        public string? Phone { get; set; }
        
        // Organisation - AVEC NOMS pour meilleure UX
        public int? TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        
        // Badge physique
        public int? BadgePhysicalNumber { get; set; }
        
        // Rôles et permissions (utilise une liste de strings pour simplicité)
        public List<string> Roles { get; set; } = new List<string>();
        
        // Statut et audit
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "System";
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
        
        // Propriétés calculées pour l'affichage
        public string FullName => $"{FirstName} {LastName}";
        public string PrimaryRole => Roles?.FirstOrDefault() ?? "Opérateur";
        public string DisplayInfo => $"{FullName} - {TeamName} ({PrimaryRole})";
        
        // Pour compatibilité
        public string Name => FullName;
        public string? UserName
        {
            get => Username;
            set => Username = value ?? string.Empty;
        }
    }
}
