using CurrencyConverterAPI.Extensions;
using CurrencyConverterAPI.Repos.IRepository;
using Newtonsoft.Json;
using System.Net.Http;

namespace CurrencyConverterAPI.Repos.Repository
{

    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private List<string> _currencies;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetCurrenciesAsync()
        {
            if (_currencies == null)
            {
                await LoadCurrenciesAsync();
            }
            return _currencies;
        }

        private async Task LoadCurrenciesAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_currencies == null)
                {
                    string baseURL = "https://api.frankfurter.app/currencies";

                    var response = await _httpClient.GetWithMultipleRetryAsync(baseURL);
                    response.EnsureSuccessStatusCode();
                    var currenciesJson = await response.Content.ReadAsStringAsync();
                    var currenciesObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(currenciesJson);
                    _currencies = currenciesObject != null ? currenciesObject.Keys.ToList() : new List<string>();

                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

}
