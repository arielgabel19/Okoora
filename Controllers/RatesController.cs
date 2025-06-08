namespace Okoora.Controllers;

using Microsoft.AspNetCore.Mvc;
using Okoora.Services;

[ApiController]
[Route("api/[controller]")]
public class RatesController : ControllerBase
{
    private readonly IRateFetcher _rateFetcher;
    private readonly ILogger<RatesController> _logger;

    public RatesController(IRateFetcher rateFetcher, ILogger<RatesController> logger)
    {
        _rateFetcher = rateFetcher;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRates()
    {
        try
        {
            _logger.LogInformation("Getting all rates");
            var rates = await _rateFetcher.GetAllRatesAsync();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all rates");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{pairName}")]
    public async Task<IActionResult> GetRate(string pairName)
    {
        try
        {
            _logger.LogInformation($"Getting rate for {pairName}");
            var rate = await _rateFetcher.GetRateByPairAsync(pairName);
            if (rate == null)
                return NotFound($"Rate for pair {pairName} not found");
            return Ok(rate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting rate for {pairName}");
            return StatusCode(500, "Internal server error");
        }
    }
}