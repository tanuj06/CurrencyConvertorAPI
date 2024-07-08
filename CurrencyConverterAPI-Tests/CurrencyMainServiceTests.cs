using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using CurrencyConverterAPI.Repos.IRepository;
using CurrencyConverterAPI.Repos.Repository;
using CurrencyConverterAPI.DataContracts;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Moq.Protected;

namespace CurrencyConverterAPI_Tests
{
    public class CurrencyMainServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<CurrencyMainService>> _loggerMock;
        private readonly Mock<ICurrencyService> _currencyServiceMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly CurrencyMainService _currencyMainService;

        public CurrencyMainServiceTests()
        {

            const string url = "https://api.frankfurter.app/";
            var mockedSection = new Mock<IConfigurationSection>();
            mockedSection.Setup(x => x.Value).Returns(url);
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x.GetSection("BaseURL"))
                .Returns(mockedSection.Object);
            _loggerMock = new Mock<ILogger<CurrencyMainService>>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _currencyMainService = new CurrencyMainService(_httpClient, _loggerMock.Object, _configurationMock.Object, _currencyServiceMock.Object);
        }

        [Fact]
        public async Task GetAllExchangeRatesForBaseCurrency_SuccessfulResponse()
        {
            // Arrange
            var currencyCode = "USD";
            var expectedBaseCurrency = "USD";
            var expectedDate = DateTime.Now.ToShortDateString();
            var expectedRates = new Dictionary<string, double> { { "EUR", 0.85 }, { "GBP", 0.76 } };
            var mockResponse = new ExchangeRateResponse
            {
                Base = expectedBaseCurrency,
                Date = DateTime.Now,
                Rates = expectedRates
            };

            var mockJson = JsonConvert.SerializeObject(mockResponse);
            var mockHttpContent = new StringContent(mockJson, Encoding.UTF8, "application/json");
            var expectedResponseJson = "{ \"base\": \"USD\", \"date\": \"2024-07-08\", \"rates\": { \"EUR\": 0.85, \"GBP\": 0.76 } }";
            var mockHttpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(expectedResponseJson),
            };

            _httpMessageHandlerMock.Protected()
         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
         .ReturnsAsync(mockHttpResponse);

            // Act
            var result = await _currencyMainService.GetAllExchangeRatesForBaseCurrency(currencyCode, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.ResponseCode);
            Assert.Equal("Success", result.ResponseMessage);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedBaseCurrency, result.Data.BaseCurrency);
            Assert.Equal(expectedDate, result.Data.FetchDate);
            Assert.Equal(expectedRates.Count, result.Data.currencies.Count);
            foreach (var rate in expectedRates)
            {
                Assert.Contains(result.Data.currencies, c => c.CurrencyCode == rate.Key && c.CurrencyValue == rate.Value);
            }
        }

        [Fact]
        public async Task GetAllExchangeRatesForBaseCurrency_ApiError()
        {
            // Arrange
            var currencyCode = "USD";
            var mockHttpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            _httpMessageHandlerMock.Protected()
         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req =>
             req.Method == HttpMethod.Get),
             ItExpr.IsAny<CancellationToken>())
         .ReturnsAsync(mockHttpResponse);

            // Act
            var result = await _currencyMainService.GetAllExchangeRatesForBaseCurrency(currencyCode, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.ResponseCode);
            Assert.Equal("Internal Server Error", result.ResponseMessage);
            Assert.NotNull(result.Data); // Ensure Data property is not null
        }

        [Fact]
        public async Task GetAllExchangeRatesForBaseCurrency_NullOrEmptyCurrencyCode()
        {
            // Act
            var result = await _currencyMainService.GetAllExchangeRatesForBaseCurrency(null, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.ResponseCode);
            Assert.Equal("Currency code must not be empty.", result.ResponseMessage);
            Assert.NotNull(result.Data); // Ensure Data property is not null
        }


        [Fact]
        public async Task GetAllExchangeRatesForBaseCurrency_ExceptionDuringApiCall()
        {
            // Arrange
            var currencyCode = "USD";

            _httpMessageHandlerMock.Protected()
         .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                           .ThrowsAsync(new HttpRequestException("Mock API request failed"));

            // Act
            var result = await _currencyMainService.GetAllExchangeRatesForBaseCurrency(currencyCode, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, result.ResponseCode);
            Assert.Equal("Internal Server Error", result.ResponseMessage);
            Assert.NotNull(result.Data); // Ensure Data property is not null
        }

    }
}