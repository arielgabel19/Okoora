namespace Okoora.Services;

using Okoora.Models;

public interface IRatePrinter
{
    Task<IEnumerable<ExchangeRate>> GetAllRatesAsync();
    Task<ExchangeRate?> GetRateByPairAsync(string pairName);
}
