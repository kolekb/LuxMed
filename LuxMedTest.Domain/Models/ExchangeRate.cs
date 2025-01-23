namespace LuxMedTest.Domain.Models
{
    public class ExchangeRate
    {
        public string Currency { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal Mid { get; set; }
    }
}
