# Currency Converter API

These 3 APIs allows you to retrieve currency conversion data from an external API service.

# Base URL
https://localhost:7070/api/CurrencyConvertor/
 
##

## 1. GetExchangeRates API

### Overview
The `GetExchangeRates` API endpoint provides the latest exchange rates for a given base currency.

### Endpoint
`GET https://localhost:7070/api/CurrencyConvertor/GetExchangeRates`

### Parameters
- `from` (query parameter, required): The base currency code for which you want to get the exchange rates. It should be a three-letter alphabetic code (e.g., `USD` for US Dollar).


### Example Request

- **Method**: GET
- **Endpoint**: `https://localhost:7070/api/CurrencyConvertor/GetExchangeRates`
- **Query Parameter**:
  - `from`: The three-letter currency code (e.g., USD, EUR).


### Success Response

1- GET - HTTP Response Code: **200**
```javascript
  HTTP/1.1 200
  Content-Type: application/json

  {
  "baseCurrency": "USD",
  "fetchDate": "05-07-2024",
  "currencies": [
    {
      "currencyCode": "AUD",
      "currencyValue": 1.4849
    },....
    ....
    {
      "currencyCode": "ZAR",
      "currencyValue": 18.2346
    }]
  }
```
## Usage

To use this API, send a GET request to the `/GetExchangeRates` endpoint with the `from` query parameter set to the desired currency code.

Example:

GET /GetExchangeRates?from=USD


## 2. Convert Currency API

### Overview
The `ConvertCurrency` API endpoint converts an amount from one currency to another based on the latest exchange rates.

### Endpoint
`GET https://localhost:7070/api/CurrencyConvertor/convertcurrency`


### Parameters
- `amount` (query parameter, optional): The amount of money to convert. It should be a positive number.
- `from` (query parameter, optional): The currency code from which the amount will be converted. It should be a three-letter alphabetic code (e.g., `USD` for US Dollar). If no currency provided it will use default currency which is `EUR` for conversion.
- `to` (query parameter, required): The currency code to which the amount will be converted. It should be a three-letter alphabetic code (e.g., `GBP` for British Pound).


### Example Request

- **Method**: GET
- **Endpoint**: `https://localhost:7070/api/CurrencyConvertor/convertcurrency`
- **Query Parameter**:
  - `amount`: The double valued positive number (e.g., 10.3).
  - `from`: The three-letter currency code (e.g., USD, EUR).
  - `to`: The three-letter currency code (e.g., USD, EUR).


### Success Response

1- GET - HTTP Response Code: **200**
```javascript
  HTTP/1.1 200
  Content-Type: application/json

  {
  "baseCurrency": "USD",
  "fetchDate": "05-07-2024",
  "currencies": [
    {
      "currencyCode": "GBP",
      "currencyValue": 7.8172
    }
   ]
  }
```
## Usage

To use this API, send a GET request to the `/convertcurrency` endpoint with the `from` , `to` query parameters set to the desired currency code and `amount` query parameter set to desired amount to convert .

Example:

GET /convertcurrency?amount=10&from=usd&to=gbp

## 3. Get Historical Data for Currency API

### Overview
The `GetHistoricalDataForCurrency` API endpoint retrieves historical exchange rates for a specified currency over a given date range.

### Endpoint
`GET https://localhost:7070/api/CurrencyConvertor/GetHistoricalDataForCurrency`

### Parameters
- `fromdate` (query parameter, required): The start date for fetching historical data in the format `YYYY-MM-DD`.
- `todate` (query parameter, optional): The end date for fetching historical data in the format `YYYY-MM-DD`. If no ToDate is provided then current date will be used.
- `amount` (query parameter, optional): The amount of money to convert. It should be a positive number. If not provided then it will treated as 1 by default.
- `from` (query parameter, optional): The currency code from which the amount will be converted. It should be a three-letter alphabetic code (e.g., `USD` for US Dollar). If no currency provided it will use default currency which is `EUR` for conversion.
- `to` (query parameter, required): The currency code to which the amount will be converted. It should be a three-letter alphabetic code (e.g., `GBP` for British Pound).
- `PageNo` (query parameter, optional): To implement pagination. By Default 1.
- `PageSize` (query parameter, optional): To return the number of records in each request. By Default 10.

### Example Request

- **Method**: GET
- **Endpoint**: `https://localhost:7070/api/CurrencyConvertor/GetHistoricalDataForCurrency`
- **Query Parameter**:
  - `fromdate`: The Start Date (e.g., 2020-10-10).
  - `todate`: The end date (e.g., 2020-10-15).
  - `amount`: The double valued positive number (e.g., 10.3).
  - `from`: The three-letter currency code (e.g., USD, EUR).
  - `to`: The three-letter currency code (e.g., USD, EUR).
  - `Pageno` : The positive integer (e.g., 2)
  - `PageSize` : The positive integer (e.g., 5)


### Success Response

1- GET - HTTP Response Code: **200**
```javascript
  HTTP/1.1 200
  Content-Type: application/json

  {
  "startDate": "04-01-1999",
  "endDate": "27-01-2020",
  "amount": 10.0,
  "baseCurrency": "EUR",
  "historicRates": [
    {
      "fetchDate": "04-01-1999",
      "rates": {
        "GBP": 7.0923
      }
    },
    {
      "fetchDate": "11-01-1999",
      "rates": {
        "GBP": 7.059
      }
    }]
  }
```
## Usage

To use this API, send a GET request to the `/GetHistoricalDataForCurrency` endpoint with the `fromdate`, `todate`, `amount`, `from` , `to`, `pageno`, `pagesize` query parameters set to the desired valid values.

Example:

`GET /GetHistoricalDataForCurrency?fromdate=xuz&todate=2020-02-01&amount=10&to=gbp&pageno=1&pagesize=2`



##

# Additional Details

## Rate Limiting

This API includes rate limiting functionality to prevent abuse and ensure fair usage of resources. Rate limiting restricts the number of requests that clients can make within a specified time period. If the rate limit is exceeded, the API returns a `429 Too Many Requests` status code.

### Implementation Details

Rate limiting is implemented using middleware in the ASP.NET Core pipeline. The middleware tracks the number of requests made by each client IP address and enforces rate limits based on predefined rules.

#### Rate Limit Configuration

The rate limit configuration is defined in the `Program.cs` file. You can adjust the following parameters to customize the rate limiting behavior:

- **Limit**: Maximum number of requests allowed per time span.
- **Period**: Time span during which the request limit applies (e.g., "10m" for 10 minutes, "1h" for 1 hour).


Example configuration:

```csharp
services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100, // Maximum number of requests per time span
            Period = "10m" // Time span (e.g., "10m" for 10 minutes)
        }
    };
});
```

### Usage

Clients should be aware of the rate limits enforced by the API and adjust their request rates accordingly. 

If a client receives a `429 Too Many Requests` response, it should wait for the rate limit window to expire before sending additional requests.


## Exceptional Handler Middleware

### Overview
The exceptional handler middleware is implemented to handle and log exceptions that occur during the execution of requests in the application. It provides a centralized approach to manage error responses and ensures that the API returns consistent and meaningful error messages to clients.

### Middleware Setup
The middleware class is created that implements `IMiddleware`. Then it is injected in the `Program.cs` file and is added to the ASP.NET Core pipeline.



# Getting Started

To get started with this project:

Please note this project is created using DotNet 7 framework.
1. Clone the repository to your local machine.
2. Open the project in Visual Studio.
3. Install the required nuget packages like NewtonSoft Json, etc.
4. Build and run the project.
5. Use a tool like Postman or a web browser to send requests to the API endpoint.

## Dependencies

This project depends on the following packages:

- `Newtonsoft.Json`: For handling JSON serialization and deserialization.
- `System.Net.Http`: For making HTTP requests to external APIs.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvement, feel free to open an issue or submit a pull request.



