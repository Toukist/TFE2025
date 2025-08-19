using System.Runtime.Serialization;

namespace Dior.Library.BO.UserInterface
{
    [DataContract]
    public class RoleDefinition
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int? ParentRoleId { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public DateTime? LastEditAt { get; set; }

        [DataMember]
        public string LastEditBy { get; set; }
    }
}
