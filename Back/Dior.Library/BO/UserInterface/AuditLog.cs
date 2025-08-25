using System.Runtime.Serialization;

namespace Dior.Library.BO.UserInterface
{
    [DataContract]
    public class AuditLog
    {
        [DataMember] public long Id { get; set; }
        [DataMember] public long UserId { get; set; }
        [DataMember] public string Action { get; set; }
        [DataMember] public string TableName { get; set; }
        [DataMember] public long RecordId { get; set; }
        [DataMember] public string? Details { get; set; }
        [DataMember] public DateTime Timestamp { get; set; }
    }
}
