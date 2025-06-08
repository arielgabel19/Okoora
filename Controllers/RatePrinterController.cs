namespace Okoora.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Web;
using Okoora.Services;

[ApiController]
[Route("api/printer")]
public class RatePrinterController : ControllerBase
{
    private readonly IRatePrinter _ratePrinter;
    private readonly ILogger<RatePrinterController> _logger;

    public RatePrinterController(IRatePrinter ratePrinter, ILogger<RatePrinterController> logger)
    {
        _ratePrinter = ratePrinter;
        _logger = logger;
    }

    [HttpGet("rates")]
    public async Task<IActionResult> GetAllPrintedRates()
    {
        try
        {
            _logger.LogInformation("Getting all printed rates");
            var rates = await _ratePrinter.GetAllRatesAsync();
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all printed rates");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("rates/{pairName}")]
    public async Task<IActionResult> GetPrintedRate([FromRoute] string pairName)
    {
        // URL decode the pair name before processing
        var decodedPairName = HttpUtility.UrlDecode(pairName);
        var rate = await _ratePrinter.GetRateByPairAsync(decodedPairName);
        
        if (rate == null)
            return NotFound($"Rate for pair {decodedPairName} not found");
            
        return Ok(rate);
    }
}