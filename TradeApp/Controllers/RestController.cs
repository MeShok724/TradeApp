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

        [HttpGet("/{pair}/{maxCount:int}")]
        public async Task<ActionResult<List<Trade>>> GetTrades(string pair, int maxCount)
        {
            var resp = await _restApi.GetNewTradesAsync(pair, maxCount);
            if (resp == null)
                return NotFound();
            return Ok(resp);
        }
    }
}
