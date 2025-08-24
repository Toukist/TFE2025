namespace Dior.Library.BO
{
    public class ContractBO
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; }
    }
}