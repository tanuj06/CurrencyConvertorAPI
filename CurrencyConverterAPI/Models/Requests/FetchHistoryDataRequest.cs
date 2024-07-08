namespace CurrencyConverterAPI.Models.Requests
{
    public class FetchHistoryDataRequest : GetExchangeRatesRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageNo { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
