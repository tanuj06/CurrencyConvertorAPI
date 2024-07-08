namespace CurrencyConverterAPI.Repos.IRepository
{
    public interface ICurrencyService
    {
        Task<List<string>> GetCurrenciesAsync();
    }
}
