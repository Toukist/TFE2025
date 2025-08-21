using System;

namespace Dior.Database.DTOs.Access
{
    public class AccessDto
    {
        public int Id { get; set; }
        public string BadgePhysicalNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}