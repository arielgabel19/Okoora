# Okoora Exchange Rate API

A .NET Core Web API that fetches and serves real-time currency exchange rates.

## Features

- Real-time exchange rate fetching from fxratesapi.com
- Automatic rate updates every 10 seconds
- Support for multiple currency pairs:
  - USD/ILS
  - EUR/ILS, EUR/USD, EUR/GBP
  - GBP/ILS, GBP/EUR
- File-based storage for rate persistence
- Swagger UI for API documentation and testing
- Comprehensive logging

## Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code
- Valid API key from fxratesapi.com

## Setup

1. Clone the repository:
```bash
git clone [repository-url]
cd Okoora
```

2. Update the API key in RateFetcher.cs:
```csharp
var apiUrl = $"https://api.fxratesapi.com/latest?api_key=YOUR_API_KEY&base={baseCurrency}&currencies={targetCurrencies}";
```

3. Build the project:
```bash
dotnet build
```

## Running the Application

1. Start the API:
```bash
dotnet run
```

2. Access the endpoints:
- Swagger UI: https://localhost:5001/swagger
- API Documentation: https://localhost:5001/swagger/v1/swagger.json

## API Endpoints

### Rate Fetcher Endpoints
- GET `/api/rates` - Get all exchange rates
- GET `/api/rates/{pairName}` - Get rate for specific currency pair

### Rate Printer Endpoints
- GET `/api/printer/rates` - Get all formatted exchange rates
- GET `/api/printer/rates/{pairName}` - Get formatted rate for specific pair

Example currency pair formats:
- EUR/USD
- EUR-USD
- EUR%2FUSD

## Project Structure

```
Okoora/
├── Controllers/
│   ├── RatesController.cs
│   └── RatePrinterController.cs
├── Models/
│   └── ExchangeRate.cs
├── Services/
│   ├── IRateFetcher.cs
│   ├── IRatePrinter.cs
│   ├── RateFetcher.cs
│   └── RatePrinter.cs
└── Program.cs
```

## Configuration

- Default HTTP port: 5000
- Default HTTPS port: 5001
- Rate update interval: 10 seconds
- Data storage: JSON file in application base directory

## Error Handling

The application includes comprehensive error handling:
- API connection failures
- File I/O errors
- Invalid currency pairs
- Rate parsing errors

All errors are logged using the built-in logging system.

## Logging

Logs are written to the console and include:
- Service startup/shutdown events
- Rate fetching operations
- File I/O operations
- API request results
- Error conditions

## Development

To modify supported currency pairs, update the `_currencyExchange` dictionary in `RateFetcher.cs`:

```csharp
Dictionary<string, string> _currencyExchange = new()
{ 
    { "USD", "ILS" },
    { "EUR", "ILS,USD,GBP" },
    { "GBP", "ILS,EUR" }
};
```

## License

[Your chosen license]
