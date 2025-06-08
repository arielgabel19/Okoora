namespace Okoora.Services;

using Okoora.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

public class RateFetcher : IRateFetcher, IHostedService
{
    private readonly HttpClient _httpClient;
    private readonly string _dataFilePath;
    private readonly ILogger<RateFetcher> _logger;
    private Timer? _timer;
    
    Dictionary<string, string> _currencyExchange = new()
    { 
        { "USD", "ILS" },
        { "EUR", "ILS,USD,GBP" },
        { "GBP", "ILS,EUR" }
    };

    public RateFetcher(IHttpClientFactory httpClientFactory, ILogger<RateFetcher> logger)
    {
        _httpClient = httpClientFactory.CreateClient() ?? 
            throw new ArgumentException("Failed to create HttpClient", nameof(httpClientFactory));
        _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rates.json");
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RateFetcher service starting...");
        
        // Fetch rates immediately on startup
        _ = Task.Run(async () => await FetchRatesAsync(), cancellationToken);
        
        // Set up timer to fetch rates every 10 seconds
        _timer = new Timer(async _ => await FetchRatesAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RateFetcher service stopping...");
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public async Task FetchRatesAsync()
    {
        _logger.LogInformation("Fetching exchange rates...");
        var rates = new List<ExchangeRate>();
        
        foreach (var pair in _currencyExchange)
        {
            var baseCurrency = pair.Key;
            var targetCurrencies = pair.Value;
            try
            {
                var apiUrl = $"https://api.fxratesapi.com/latest?api_key=fxr_live_43ba481679465aad5f8be38a252c891b7a02&base={baseCurrency}&currencies={targetCurrencies}";
                var response = await _httpClient.GetAsync(apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<FxRatesApiResponse>();
                    if (apiResponse?.Success == true)
                    {
                        foreach (var rate in apiResponse.Rates)
                        {
                            rates.Add(new ExchangeRate
                            {
                                BaseCurrency = baseCurrency,
                                QuoteCurrency = rate.Key,
                                Rate = rate.Value,
                                LastUpdateTime = DateTime.UtcNow
                            });
                        }
                    }
                }
                else
                {
                    _logger.LogWarning($"API request failed with status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching rates for {baseCurrency}: {ex.Message}");
            }
        }

        if (rates.Any())
        {
            try
            {
                var json = JsonSerializer.Serialize(rates, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_dataFilePath, json);
                _logger.LogInformation($"Successfully saved {rates.Count} exchange rates to file");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving rates to file: {ex.Message}");
            }
        }
        else
        {
            _logger.LogWarning("No rates were fetched");
        }
    }

    public async Task<IEnumerable<ExchangeRate>> GetAllRatesAsync()
    {
        try
        {
            if (!File.Exists(_dataFilePath))
            {
                _logger.LogWarning($"Rates file not found at: {_dataFilePath}");
                return Enumerable.Empty<ExchangeRate>();
            }

            var json = await File.ReadAllTextAsync(_dataFilePath);
            var rates = JsonSerializer.Deserialize<List<ExchangeRate>>(json) ?? new List<ExchangeRate>();
            _logger.LogInformation($"Retrieved {rates.Count} rates from file");
            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reading rates from file: {ex.Message}");
            return Enumerable.Empty<ExchangeRate>();
        }
    }

    public async Task<ExchangeRate?> GetRateByPairAsync(string pairName)
    {
        var rates = await GetAllRatesAsync();
        return rates.FirstOrDefault(r => r.PairName.Equals(pairName, StringComparison.OrdinalIgnoreCase));
    }
}