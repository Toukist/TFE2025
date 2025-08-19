using System.Runtime.Serialization;

namespace Dior.Library.BO.UserInterface
{
    [DataContract]
    public class AccessCompetency
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? ParentId { get; set; }

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
