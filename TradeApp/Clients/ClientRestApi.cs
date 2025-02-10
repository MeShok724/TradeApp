using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using TradeApp.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TradeApp.Clients
{
    public class ClientRestApi : ITestConnectorRest
    {
        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, 
            int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, 
            long? count = 0)
        {
            var httpClient = new HttpClient();
            string timeFrame = ConvertTime(periodInSec);

            string candleQuerry = $"trade:{timeFrame}:{pair}";
            Console.WriteLine($"Pair ---> {pair}");
            Console.WriteLine($"CandleQuerry ---> {candleQuerry}");

            UriBuilder uriBuilder = new UriBuilder($"https://api-pub.bitfinex.com/v2/candles/{candleQuerry}/hist");
            var querryParams = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (from != null)
            {
                DateTimeOffset date = (DateTimeOffset)from;
                querryParams["start"] = Convert.ToString(date.ToUnixTimeMilliseconds());
            }
            if (to != null)
            {
                DateTimeOffset date = (DateTimeOffset)to;
                querryParams["end"] = Convert.ToString(date.ToUnixTimeMilliseconds());
            }
            if (count != null & count != 0)
            {
                querryParams["limit"] = count.ToString();
            }
            uriBuilder.Query = querryParams.ToString();
            string url = uriBuilder.ToString();
            Console.WriteLine(url);

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResp = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResp);
                var candles = JsonSerializer.Deserialize<List<List<JsonElement>>>(jsonResp);
                if (candles == null)
                {
                    Console.WriteLine("Error deserialize");
                    return null;
                }
                List<Candle> candleList = new List<Candle>();
                foreach (var currCandle in candles)
                {
                    Candle candle = new Candle()
                    {
                        Pair = pair,
                        OpenPrice = currCandle[1].GetInt64(),
                        HighPrice = currCandle[3].GetInt64(),
                        LowPrice = currCandle[4].GetInt64(),
                        ClosePrice = currCandle[2].GetInt64(),
                        TotalVolume = currCandle[2].GetDecimal(),
                        OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(currCandle[0].GetInt64())
                    };
                    candleList.Add(candle);
                }
                return candleList;
            }
            else
            {
                return null;
            }
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

        private string ConvertTime(int periodInSec)
        {
            string timeFrame;
            if (periodInSec <= 60)
                timeFrame = "1m";
            else if (periodInSec <= 60 * 5)
                timeFrame = "5m";
            else if (periodInSec <= 60 * 15)
                timeFrame = "15m";
            else if (periodInSec <= 60 * 30)
                timeFrame = "30m";
            else if (periodInSec <= 60 * 60)
                timeFrame = "1h";
            else if (periodInSec <= 60 * 60 * 3)
                timeFrame = "3h";
            else if (periodInSec <= 60 * 60 * 6)
                timeFrame = "6h";
            else if (periodInSec <= 60 * 60 * 12)
                timeFrame = "12h";
            else if (periodInSec <= 60 * 60 * 24)
                timeFrame = "1D";
            else if (periodInSec <= 60 * 60 * 24 * 7)
                timeFrame = "1W";
            else if (periodInSec <= 60 * 60 * 24 * 14)
                timeFrame = "14D";
            else
                timeFrame = "1M";

            return timeFrame;
        }
    }
}
