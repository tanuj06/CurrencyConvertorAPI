using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Models.Requests;
using CurrencyConverterAPI.Models.Responses;
using Newtonsoft.Json.Linq;

namespace CurrencyConverterAPI.Repos.IRepository
{
    public interface ICurrencyMainService
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateCurrencyCodeAsync(GetExchangeRatesRequest request, bool CheckToCurrency = true);

        Task<ApiResponse<CurrencyRatesResponse>> GetAllExchangeRatesForBaseCurrency(string CurrencyCode, CancellationToken token);

        Task<ApiResponse<CurrencyRatesResponse>> GetSelectedExchangeRatesForBaseCurrency(double? amount, string? baseCurrencyCode, string? childCurrencyCode, CancellationToken token);

        Task<ApiResponse<HistoryPerCurrencyResponse>> GetHistoricalDataForCurrency(FetchHistoryDataRequest request, CancellationToken token);
    }
}
