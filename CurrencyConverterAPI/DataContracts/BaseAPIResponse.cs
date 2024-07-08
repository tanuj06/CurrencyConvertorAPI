namespace CurrencyConverterAPI.DataContracts
{
    public class BaseAPIResponse
    {
        public double Amount { get; set; }
        public string Base { get; set; }
        public DateTime Date { get; set; }
    }

}
