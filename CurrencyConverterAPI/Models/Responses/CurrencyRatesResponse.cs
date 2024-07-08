namespace CurrencyConverterAPI.Models.Responses
{
    public class CurrencyRatesResponse
    {
        public CurrencyRatesResponse()
        {
            currencies = new List<CurrencyRates>();
        }
        public string BaseCurrency { get; set; }

        public string FetchDate { get; set; }

        public List<CurrencyRates> currencies { get; set; }
    }
}
