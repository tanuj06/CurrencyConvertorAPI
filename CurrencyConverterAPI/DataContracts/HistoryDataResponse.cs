namespace CurrencyConverterAPI.DataContracts
{
    public class HistoryDataResponse : BaseAPIResponse
    {
        public string start_date { get; set; }
        public string end_date { get; set; }
        public Dictionary<string, Dictionary<string, double>> Rates { get; set; }
    }
}
