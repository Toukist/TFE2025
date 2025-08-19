namespace Dior.Library.BO
{
    public class Payslip
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal Deductions { get; set; } = 0;
        public decimal Bonus { get; set; } = 0;
        public string FileUrl { get; set; } = string.Empty;
        public bool IsSent { get; set; } = false;
        public DateTime? SentDate { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public string GeneratedBy { get; set; } = string.Empty;
    }
}