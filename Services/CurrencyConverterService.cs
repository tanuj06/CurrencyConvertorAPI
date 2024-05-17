using CurrencyConverterAPI.Extensions;
using CurrencyConverterAPI.Interfaces;
using CurrencyConverterAPI.Models;
using System.Net;

namespace CurrencyConverterAPI.Services
{
    public class CurrencyConverterService: ICurrencyConverter
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
            CurrencyCode = valdiateCurrencyCode(CurrencyCode);
            if (CurrencyCode == null)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessage = "Currency code must be three alphabetic characters." };
            }
            string baseURL = "https://api.frankfurter.app/latest";
            if(!string.IsNullOrEmpty(CurrencyCode) )
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

        private string? valdiateCurrencyCode(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode) || currencyCode.Length != 3)
            {
                return null;
            }

            foreach (char c in currencyCode)
            {
                if (!char.IsLetter(c))
                {
                    return null;
                }
            }

            return currencyCode.ToUpper();
        }
    }
}
