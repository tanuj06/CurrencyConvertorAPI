using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Interfaces
{
    public interface ICurrencyConverter
    {
        Task<ApiResponse> GetDataFromFrankFurterAsync(string CurrencyCode, CancellationToken token);
    }
}
