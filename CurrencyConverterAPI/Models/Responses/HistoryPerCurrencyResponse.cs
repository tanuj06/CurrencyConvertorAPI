namespace CurrencyConverterAPI.Models.Responses
{
    public class HistoryPerCurrencyResponse
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public double Amount { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public List<HistoricRates> historicRates { get; set; } = new List<HistoricRates>();
    }

    public class HistoricRates
    {
        public string FetchDate { get; set; }
        public Dictionary<string, double>? Rates { get; set; }
    }
}
