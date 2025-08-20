using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Dior.Library.BO.UserInterface
{
    [DataContract]
    [Table("ROLE_DEFINITION_PRIVILEGE")]
    public class RoleDefinitionPrivilege
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int RoleDefinitionId { get; set; }
        [DataMember]
        public int PrivilegeId { get; set; }
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