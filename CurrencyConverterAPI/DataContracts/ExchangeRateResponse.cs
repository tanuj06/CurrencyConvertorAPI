namespace CurrencyConverterAPI.DataContracts
{
    public class ExchangeRateResponse : BaseAPIResponse
    {
        public Dictionary<string, double> Rates { get; set; }
    }
}
