namespace Okoora.Services;

using Okoora.Models;

public interface IRateFetcher
{
    Task FetchRatesAsync();
    Task<IEnumerable<ExchangeRate>> GetAllRatesAsync();
    Task<ExchangeRate?> GetRateByPairAsync(string pairName);
}
