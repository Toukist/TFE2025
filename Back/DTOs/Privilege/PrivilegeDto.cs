using System;

namespace Dior.Database.DTOs.Privilege
{
    public class PrivilegeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}