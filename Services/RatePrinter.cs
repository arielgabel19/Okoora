namespace Okoora.Services;

using System.Web;
using System.ComponentModel;
using Newtonsoft.Json;
using Okoora.Models;

public class RatePrinter : IRatePrinter
{
    private readonly IRateFetcher _rateFetcher;
    private readonly ILogger<RatePrinter> _logger;

    public RatePrinter(IRateFetcher rateFetcher, ILogger<RatePrinter> logger)
    {
        _rateFetcher = rateFetcher;
        _logger = logger;
    }

    public async Task<IEnumerable<ExchangeRate>> GetAllRatesAsync()
    {
        var rates = await _rateFetcher.GetAllRatesAsync();
        _logger.LogInformation($"RatePrinter retrieved {rates.Count()} rates");
        return rates;
    }

    public async Task<ExchangeRate?> GetRateByPairAsync(string pairName)
    {
        // Decode URL encoded string and normalize the pair format
        string normalizedPairName = HttpUtility.UrlDecode(pairName)
            .Replace("-", "/")
            .Replace("%2F", "/")
            .Trim();

        var rate = await _rateFetcher.GetRateByPairAsync(normalizedPairName);
        _logger.LogInformation(rate != null 
            ? $"RatePrinter found rate for {normalizedPairName}: {rate.Rate}"
            : $"RatePrinter - Rate not found for {normalizedPairName}");
        return rate;
    }
}