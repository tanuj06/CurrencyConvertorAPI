using CurrencyConverterAPI.DataContracts;
using CurrencyConverterAPI.Extensions;
using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Models.Requests;
using CurrencyConverterAPI.Models.Responses;
using CurrencyConverterAPI.Repos.IRepository;
using Newtonsoft.Json;
using System.Net;

namespace CurrencyConverterAPI.Repos.Repository
{
    public class CurrencyMainService : ICurrencyMainService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyMainService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICurrencyService _currencyService;
        private string baseURL;

        /// <summary>
        /// injecting http client dependency in constructor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public CurrencyMainService(HttpClient httpClient, ILogger<CurrencyMainService> logger, IConfiguration configuration, ICurrencyService currencyService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _currencyService = currencyService;
            baseURL = _configuration.GetValue<string>("BaseURL") ?? string.Empty;
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateCurrencyCodeAsync(GetExchangeRatesRequest request, bool CheckToCurrency = true)
        {
            if (request == null || string.IsNullOrEmpty(request.From) && string.IsNullOrEmpty(request.To))
            {
                return (false, "Failed to validate currency code. No currency code provided");
            }

            if (!string.IsNullOrEmpty(request.From))
            {
                var (isFromValid, fromErrorMessage) = await ValidateCurrencyCode(request.From);
                if (!isFromValid)
                {
                    return (false, fromErrorMessage);
                }
            }

            if (CheckToCurrency && !string.IsNullOrEmpty(request.To))
            {
                var (isToValid, toErrorMessage) = await ValidateCurrencyCode(request.To);
                if (!isToValid)
                {
                    return (false, toErrorMessage);
                }
            }

            return (true, string.Empty);
        }

        private async Task<(bool isValid, string errorMessage)> ValidateCurrencyCode(string currencyCode)
        {
            //for empty currency code
            if (string.IsNullOrEmpty(currencyCode))
            {
                return (false, "Bad input: " + currencyCode + " :Currency code must not be empty.");
            }

            //for checking length of currency code
            if (currencyCode.Length != 3)
            {
                return (false, "Bad input: " + currencyCode + " :Currency code must be three characters long.");
            }

            //for checking non-alphabets in currency code
            foreach (char c in currencyCode)
            {
                if (!char.IsLetter(c))
                {
                    return (false, "Bad input: " + currencyCode + " :Currency code must contain only alphabetic characters.");
                }
            }

            //fetch all currencies from API and validate currencycode in the fetched list
            var currencies = await _currencyService.GetCurrenciesAsync();
            if (!currencies.Contains(currencyCode.ToUpper()))
            {
                return (false, "Bad input: " + currencyCode + " :Invalid currency code.");
            }

            return (true, string.Empty); // Valid currency code
        }

        public async Task<ApiResponse<CurrencyRatesResponse>> GetAllExchangeRatesForBaseCurrency(string CurrencyCode, CancellationToken token)
        {
            //string baseURL = _configuration.GetValue<string>("BaseURL") ?? string.Empty;
            baseURL += "latest?from=" + CurrencyCode;

            var httpResponse = await _httpClient.GetWithMultipleRetryAsync(baseURL);
            httpResponse.EnsureSuccessStatusCode();
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<ExchangeRateResponse>(responseJson);
            if (httpResponse.StatusCode == HttpStatusCode.OK && responseData != null)
            {
                CurrencyRatesResponse currencyRatesPerCurrency = new CurrencyRatesResponse
                {
                    BaseCurrency = responseData.Base,
                    FetchDate = responseData.Date.ToShortDateString(),
                    currencies = responseData.Rates.Select(rate => new CurrencyRates
                    {
                        CurrencyCode = rate.Key,
                        CurrencyValue = rate.Value
                    }).ToList()
                };
                return new ApiResponse<CurrencyRatesResponse>
                {
                    ResponseCode = (int)httpResponse.StatusCode,
                    ResponseMessage = "Success",
                    Data = currencyRatesPerCurrency
                };
            }
            return new ApiResponse<CurrencyRatesResponse>
            {
                ResponseCode = (int)httpResponse.StatusCode,
                ResponseMessage = "Internal Server Error",
                Data = new CurrencyRatesResponse()
            };
        }

        public async Task<ApiResponse<CurrencyRatesResponse>> GetSelectedExchangeRatesForBaseCurrency(double? amount, string? baseCurrencyCode, string? childCurrencyCode, CancellationToken token)
        {
            //string baseURL = _configuration.GetValue<string>("BaseURL") ?? string.Empty;
            HashSet<string> RestrictedCurrencies = new HashSet<string> { "TRY", "PLN", "THB", "MXN" };
            if (!string.IsNullOrEmpty(childCurrencyCode) &&
                RestrictedCurrencies.Contains(childCurrencyCode.ToUpper())){
                return new ApiResponse<CurrencyRatesResponse>
                {
                    ResponseCode = StatusCodes.Status400BadRequest,
                    ResponseMessage = "Restricted Currency Code passed",
                    Data = new CurrencyRatesResponse()
                };
            }
            baseURL += "latest"
                + (amount != null && amount > 0.0 ? ("?amount=" + amount + "&") : "?")
                + (!string.IsNullOrEmpty(baseCurrencyCode) ? ("from=" + baseCurrencyCode + "&") : "")
                + (!string.IsNullOrEmpty(childCurrencyCode) ? ("to=" + childCurrencyCode) : "");

            var httpResponse = await _httpClient.GetWithMultipleRetryAsync(baseURL);
            httpResponse.EnsureSuccessStatusCode();
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<ExchangeRateResponse>(responseJson);
            if (responseData != null)
            {
                CurrencyRatesResponse currencyRatesPerCurrency = new CurrencyRatesResponse
                {
                    BaseCurrency = responseData.Base,
                    FetchDate = responseData.Date.ToShortDateString(),
                    currencies = responseData.Rates.Select(rate => new CurrencyRates
                    {
                        CurrencyCode = rate.Key,
                        CurrencyValue = rate.Value
                    }).ToList()
                };
                return new ApiResponse<CurrencyRatesResponse>
                {
                    ResponseCode = (int)httpResponse.StatusCode,
                    ResponseMessage = "Success",
                    Data = currencyRatesPerCurrency
                };
            }
            return new ApiResponse<CurrencyRatesResponse>
            {
                ResponseCode = StatusCodes.Status500InternalServerError,
                ResponseMessage = "Internal Server Error",
                Data = new CurrencyRatesResponse()
            };
        }

        public async Task<ApiResponse<HistoryPerCurrencyResponse>> GetHistoricalDataForCurrency(FetchHistoryDataRequest request, CancellationToken token)
        {
            int pageno = request.PageNo;
            int pagesize = request.PageSize;

            int skip = (pageno - 1) * pagesize;

            //string baseURL = _configuration.GetValue<string>("BaseURL") ?? string.Empty;

            baseURL += request.FromDate.Date.ToString("yyyy-MM-dd") + ".." + request.ToDate?.Date.ToString("yyyy-MM-dd")
                + (request.Amount != null && request.Amount > 0.0 ? ("?amount=" + request.Amount + "&") : "?")
                + (!string.IsNullOrEmpty(request.From) ? ("from=" + request.From + "&") : "")
                + (!string.IsNullOrEmpty(request.To) ? ("to=" + request.To) : "");


            var httpResponse = await _httpClient.GetWithMultipleRetryAsync(baseURL);
            httpResponse.EnsureSuccessStatusCode();
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<HistoryDataResponse>(responseJson);
            if (responseData != null)
            {
                var paginatedRates = responseData.Rates.Select(rate => new HistoricRates
               {
                   FetchDate = DateTime.Parse(rate.Key).ToShortDateString(),
                   Rates = rate.Value
               })
               .Skip(skip)
               .Take(pagesize)
               .ToList();


                HistoryPerCurrencyResponse historyPerCurrencyResponse = new HistoryPerCurrencyResponse
                {
                    BaseCurrency = responseData.Base,
                    StartDate = DateTime.Parse(responseData.start_date).ToShortDateString(),
                    EndDate = DateTime.Parse(responseData.end_date).ToShortDateString(),
                    Amount = responseData.Amount,
                    historicRates = paginatedRates
                };
                return new ApiResponse<HistoryPerCurrencyResponse>
                {
                    ResponseCode = (int)httpResponse.StatusCode,
                    ResponseMessage = "Success",
                    Data = historyPerCurrencyResponse
                };
            }
            return new ApiResponse<HistoryPerCurrencyResponse>
            {
                ResponseCode = StatusCodes.Status500InternalServerError,
                ResponseMessage = "Internal Server Error",
                Data = new HistoryPerCurrencyResponse()
            };
        }
    }
}

