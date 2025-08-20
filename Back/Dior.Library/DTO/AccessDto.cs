
using Dior.Library.Entities;

namespace Dior.Library.DTO
{

    public class AccessDto
    {
        public int Id { get; set; }
        public string BadgePhysicalNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public AccessDto(Access access)
        {

            Id = access.Id;
            BadgePhysicalNumber = access.BadgePhysicalNumber;
            IsActive = access.IsActive;
            CreatedAt = access.CreatedAt;
            CreatedBy = access.CreatedBy;
        }

        public AccessDto()
        {

        }
    }

}
