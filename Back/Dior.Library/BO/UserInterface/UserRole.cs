using System.Runtime.Serialization;

namespace Dior.Library.BO.UserInterface
{
    [DataContract]
    public class UserRole
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int RoleDefinitionId { get; set; }

        [DataMember]
        public DateTime? ExpiresAt { get; set; }

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
