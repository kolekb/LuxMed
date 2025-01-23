namespace LuxMedTest.Application.Dtos
{
    public class ExchangeRateDto
    {
        public string Currency { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal Rate { get; set; }  
    }
}
