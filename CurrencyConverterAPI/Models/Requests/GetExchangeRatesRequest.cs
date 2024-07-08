namespace CurrencyConverterAPI.Models.Requests
{
    public class GetExchangeRatesRequest
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public double? Amount { get; set; }
    }
}
