using CurrencyConverterAPI.Extensions;
using CurrencyConverterAPI.Interfaces;
using CurrencyConverterAPI.Models;
using Newtonsoft.Json;
using System.Net;

namespace CurrencyConverterAPI.Services
{
    public class CurrencyConverterService : ICurrencyConverter
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyConverterService> _logger;

        /// <summary>
        /// injecting http client dependency in constructor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public CurrencyConverterService(HttpClient httpClient, ILogger<CurrencyConverterService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse> GetDataFromFrankFurterAsync(string CurrencyCode, CancellationToken token)
        {
            var (validatedCurrencyCode, errorMessage) = await ValidateCurrencyCodeAsync(CurrencyCode);
            if (validatedCurrencyCode == null)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessage = errorMessage };
            }

            CurrencyCode = validatedCurrencyCode;

            string baseURL = "https://api.frankfurter.app/latest";
            if (!string.IsNullOrEmpty(CurrencyCode))
            {
                baseURL += "?from=" + CurrencyCode;
            }
            try
            {
                var response = await _httpClient.GetWithMultipleRetryAsync(baseURL);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                return new ApiResponse { IsSuccess = true, Data = responseData };
            }
            catch (HttpRequestException ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessage = "An error occurred while fetching data from the FrankFurter API: " + ex.Message };
            }
        }

        private async Task<(string?, string)> ValidateCurrencyCodeAsync(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                return (null, "Currency code must not be empty.");
            }

            if (currencyCode.Length != 3)
            {
                return (null, "Currency code must be three characters long.");
            }

            foreach (char c in currencyCode)
            {
                if (!char.IsLetter(c))
                {
                    return (null, "Currency code must contain only alphabetic characters.");
                }
            }

            var currencies = await GetAllCurrenciesFromFrankFurterAsync();

            if (!currencies.Contains(currencyCode.ToUpper()))
            {
                return (null, "Invalid currency code.");
            }
            return (currencyCode.ToUpper(), string.Empty);
        }

        private async Task<List<string>> GetAllCurrenciesFromFrankFurterAsync()
        {
            string baseURL = "https://api.frankfurter.app/currencies";

            try
            {
                var response = await _httpClient.GetWithMultipleRetryAsync(baseURL);
                response.EnsureSuccessStatusCode();
                var currenciesJson = await response.Content.ReadAsStringAsync();
                var currenciesObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(currenciesJson);

                var currencies = currenciesObject.Keys.ToList();
                return currencies;
            }
            catch (HttpRequestException ex)
            {
                return new List<string>();
            }
        }

    }
}
