# Currency Converter API

This API allows you to retrieve currency conversion data from an external API service.

## Endpoint

The API exposes the following endpoint:

GET /GetLatestExchangeRates?CurrencyCode=<currency-code>


## Request

- **Method**: GET
- **Endpoint**: `/GetLatestExchangeRates`
- **Query Parameter**:
  - `CurrencyCode`: The three-letter currency code (e.g., USD, EUR).

## Response

The API returns the following responses:

- **Success Response**:
  - **Status Code**: 200 OK
  - **Content Type**: application/json
  - **Body**: The response body contains the currency conversion data.

- **Error Response**:
  - **Status Code**: 400 Bad Request
  - **Content Type**: text/plain
  - **Body**: Error message describing the issue with the request. For example, if an invalid currency code is provided.

## Usage

To use this API, send a GET request to the `/GetLatestExchangeRates` endpoint with the `CurrencyCode` query parameter set to the desired currency code.

Example:

GET /getdatafromexternalapi?CurrencyCode=USD

## Rate Limiting

This API includes rate limiting functionality to prevent abuse and ensure fair usage of resources. Rate limiting restricts the number of requests that clients can make within a specified time period. If the rate limit is exceeded, the API returns a `429 Too Many Requests` status code.

### Implementation Details

Rate limiting is implemented using middleware in the ASP.NET Core pipeline. The middleware tracks the number of requests made by each client IP address and enforces rate limits based on predefined rules.

#### Rate Limit Configuration

The rate limit configuration is defined in the `Startup.cs` file. You can adjust the following parameters to customize the rate limiting behavior:

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


## Getting Started

To get started with this project:

Please note this project is created using DotNet 7 framework.
1. Clone the repository to your local machine.
2. Open the project in Visual Studio.
3. Build and run the project.
4. Use a tool like Postman or a web browser to send requests to the API endpoint.

## Dependencies

This project depends on the following packages:

- `Newtonsoft.Json`: For handling JSON serialization and deserialization.
- `System.Net.Http`: For making HTTP requests to external APIs.

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvement, feel free to open an issue or submit a pull request.





