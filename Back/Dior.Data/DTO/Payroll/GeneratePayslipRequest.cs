namespace Dior.Library.DTO.Payroll
{
    public class GeneratePayslipRequest
    {
        public List<long> UserIds { get; set; } = new();
        public int? TeamId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
