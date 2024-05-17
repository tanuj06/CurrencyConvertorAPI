using CurrencyConverterAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterAPI.Controllers
{
    [Route("api/[controller]")]
    public class CurrencyConvertorController : Controller
    {
        public readonly ICurrencyConverter _currencyConverter;

        public CurrencyConvertorController(ICurrencyConverter currencyConverter)
        {
                _currencyConverter = currencyConverter;
        }

        /// <summary>
        /// method to extract all data from frankfurter api
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLatestExchangeRates")]
        public async Task<IActionResult> GetDataFromExternalAPI([FromQuery] string? CurrencyCode, CancellationToken token)
        { 
            var result = await _currencyConverter.GetDataFromFrankFurterAsync(CurrencyCode ?? string.Empty, token);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
