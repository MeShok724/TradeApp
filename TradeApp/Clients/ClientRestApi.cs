using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using TradeApp.Core;

namespace TradeApp.Clients
{
    public class ClientRestApi : ITestConnectorRest
    {
        public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            var httpClient = new HttpClient();
            string url = $"https://api-pub.bitfinex.com/v2/trades/{pair}/hist?limit={maxCount}";
            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResp = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResp);
                var trades = JsonSerializer.Deserialize<List<List<JsonElement>>>(jsonResp);
                if (trades == null)
                {
                    Console.WriteLine("Error deserialize");
                    return null;
                }

                List<Trade> tradeList = new List<Trade>();
                foreach (var currTrade in trades)
                {
                    Trade trade = new Trade()
                    {
                        Pair = pair,
                        Id = Convert.ToString(currTrade[0].GetInt64()),
                        Time = DateTimeOffset.FromUnixTimeMilliseconds(currTrade[1].GetInt64()),
                        Amount = Math.Abs(currTrade[2].GetDecimal()),
                        Price = currTrade[3].GetDecimal(),
                        Side = currTrade[2].GetDecimal() > 0 ? "buy" : "sell"
                    };
                    tradeList.Add(trade);
                }
                return tradeList;
            }
            else
            {
                return null;
            }
        }
    }
}
