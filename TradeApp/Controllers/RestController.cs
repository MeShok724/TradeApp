using Microsoft.AspNetCore.Mvc;
using TradeApp.Clients;
using TradeApp.Core;

namespace TradeApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestController : Controller
    {
        private ClientRestApi _restApi = new ClientRestApi();

        [HttpGet("Trade/{pair}/{maxCount:int}")]
        public async Task<ActionResult<List<Trade>>> GetTrades(string pair, int maxCount)
        {
            var resp = await _restApi.GetNewTradesAsync(pair, maxCount);
            if (resp == null)
                return NotFound();
            return Ok(resp);
        }

        [HttpGet("Candle/{pair}")]
        public async Task<ActionResult<List<Candle>>> GetCandles(string pair, 
            [FromQuery] int periodInSec, [FromQuery] DateTimeOffset? from = null, 
            [FromQuery] DateTimeOffset? to = null, [FromQuery]long? count = 0)
        {
            var resp = await _restApi.GetCandleSeriesAsync(pair, periodInSec, from, to, count);
            if (resp == null)
                return NotFound();
            return Ok(resp);
        }
    }
}
