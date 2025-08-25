using System;

namespace Dior.Data.DTO.Access
{
    /// <summary>
    /// DTO pour les acc�s
    /// </summary>
    public class AccessDto
    {
        public int Id { get; set; }
        public string BadgePhysicalNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }
    }

    /// <summary>
    /// DTO pour cr�er un acc�s
    /// </summary>
    public class CreateAccessDto
    {
        public string BadgePhysicalNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre � jour un acc�s
    /// </summary>
    public class UpdateAccessDto
    {
        public string? BadgePhysicalNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}