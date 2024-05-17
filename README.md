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





