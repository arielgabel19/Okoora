namespace Okoora.Models;

public class FxRatesApiResponse
{
    public bool Success { get; set; }
    public Dictionary<string, decimal> Rates { get; set; } = new();
}