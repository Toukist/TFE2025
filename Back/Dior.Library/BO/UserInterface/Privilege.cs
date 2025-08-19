using System.Runtime.Serialization;

namespace Dior.Library.BO.UserInterface
{
    [DataContract]
    public class Privilege
    {
        /// <summary>
        /// Index autoincremented
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Privilege name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Privilege description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the privilege is active
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }
    }
}
