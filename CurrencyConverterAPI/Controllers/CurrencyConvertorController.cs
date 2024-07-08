using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Models.Requests;
using CurrencyConverterAPI.Repos.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CurrencyConverterAPI.Controllers
{
    [Route("api/[controller]")]
    public class CurrencyConvertorController : Controller
    {
        public readonly ICurrencyMainService _currencyConverter;

        public CurrencyConvertorController(ICurrencyMainService currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        /// <summary>
        /// method to extract latest exchange rates of all currencies from frankfurter api for a particular base currency
        /// </summary>
        /// <returns>exchange rates of all currencies</returns>
        [HttpGet]
        [Route("GetExchangeRates")]
        public async Task<IActionResult> GetAllExchangeRatesForBaseCurrency([FromQuery] string From, CancellationToken token)
        {
            bool CheckToCurrency = false;
            GetExchangeRatesRequest request = new GetExchangeRatesRequest();
            if (!string.IsNullOrEmpty(From))
            {
                request.From = From;
            }
            #region validationCheck
            var (isValidCurrencyCode, errorMessage) = await _currencyConverter.ValidateCurrencyCodeAsync(request, CheckToCurrency);
            if (!isValidCurrencyCode)
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = StatusCodes.Status400BadRequest, ResponseMessage = errorMessage, Data = "[]" });
            }
            #endregion validationCheck

            var result = await _currencyConverter.GetAllExchangeRatesForBaseCurrency(request.From.ToUpper(), token);
            if (result.ResponseCode != StatusCodes.Status200OK)
            {
                return BadRequest(result.ResponseMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// method to extract latest exchange rates of requested currencies from frankfurter api for a particular base currency
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns>exchange rates for requested currencies</returns>
        [HttpGet]
        [Route("ConvertCurrency")]
        public async Task<IActionResult> GetSelectedExchangeRatesForBaseCurrency([FromQuery] GetExchangeRatesRequest request, CancellationToken token)
        {
            #region validationCheck
            var (isValidCurrencyCode, errorMessage) = await _currencyConverter.ValidateCurrencyCodeAsync(request);
            if (!isValidCurrencyCode)
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = StatusCodes.Status400BadRequest, ResponseMessage = errorMessage, Data = "[]" });
            }
            #endregion validationCheck

            var result = await _currencyConverter.GetSelectedExchangeRatesForBaseCurrency(request.Amount ?? 0.0, request.From, request.To, token);
            if (result.ResponseCode != StatusCodes.Status200OK)
            {
                return BadRequest(result.ResponseMessage);
            }
            return Ok(result.Data);
        }

        /// <summary>
        /// Get Historical Exchange Rates between requested dates for a particular base currency
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="Amount"></param>
        /// <param name="token"></param>
        /// <returns>historical exchange rates for all currencies or requested currencies</returns>
        [HttpGet]
        [Route("GetHistoricalDataForCurrency")]
        public async Task<IActionResult> GetHistoricalDataForCurrency( [FromQuery] FetchHistoryDataRequest request, CancellationToken token)
        {

            request.ToDate = request.ToDate ?? DateTime.Today;

            #region validationCheck
            var (isValidCurrencyCode, errorMessage) = await _currencyConverter.ValidateCurrencyCodeAsync(request);
            if (!isValidCurrencyCode)
            {
                return BadRequest(new ApiResponse<string> { ResponseCode = StatusCodes.Status400BadRequest, ResponseMessage = errorMessage, Data = "[]" });
            }
            #endregion validationCheck



            var result = await _currencyConverter.GetHistoricalDataForCurrency(request, token);
            if (result.ResponseCode != StatusCodes.Status200OK)
            {
                return BadRequest(result.ResponseMessage);
            }
            return Ok(result.Data);
        }

    }
}
