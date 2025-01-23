using System.Xml.Serialization;

namespace LuxMedTest.Infrastructure.Clients.Models
{
    [XmlRoot("ExchangeRatesTable")]
    public class ExchangeRateTable
    {
        [XmlElement("Table")]
        public string Table { get; set; } = string.Empty;

        [XmlElement("No")]
        public string No { get; set; } = string.Empty;

        [XmlElement("EffectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;

        [XmlElement("Rate")]
        public List<ExchangeRateEntry> Rates { get; set; } = new List<ExchangeRateEntry>();
    }
}
