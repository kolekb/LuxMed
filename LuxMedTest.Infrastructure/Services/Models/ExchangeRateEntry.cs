using System.Xml.Serialization;

namespace LuxMedTest.Infrastructure.Clients.Models
{
    public class ExchangeRateEntry
    {
        [XmlElement("Currency")]
        public string Currency { get; set; } = string.Empty;

        [XmlElement("Code")]
        public string Code { get; set; } = string.Empty;

        [XmlElement("Mid")]
        public decimal Mid { get; set; }
    }
}
