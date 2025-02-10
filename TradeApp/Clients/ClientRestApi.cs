using TradeApp.Core;

namespace TradeApp.Clients
{
    public class ClientRestApi : ITestConnectorRest
    {
        public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            throw new NotImplementedException();
        }
    }
}
