using Dior.Library.Entities;

namespace Dior.Library.DTO
{

    public class AccessDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BadgeNumber { get; set; }
        public string? BadgePhysicalNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastEditAt { get; set; }
        public string? LastEditBy { get; set; }

        public AccessDto(Access access)
        {

            Id = access.Id;
            Name = access.Name;
            Description = access.Description;
            BadgeNumber = access.BadgeNumber;
            BadgePhysicalNumber = access.BadgePhysicalNumber;
            IsActive = access.IsActive;
            CreatedAt = access.CreatedAt;
            CreatedBy = access.CreatedBy;
            LastEditAt = access.LastEditAt;
            LastEditBy = access.LastEditBy;
        }

        public AccessDto()
        {

        }
    }

}
