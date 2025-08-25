using System;

namespace Dior.Library.Entities
{
    public class Access
    {
        public int Id { get; set; }
        public string BadgePhysicalNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }
}
