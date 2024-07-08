namespace CurrencyConverterAPI.Models
{
    public class ApiResponse<T>
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public T Data { get; set; }
    }

}
