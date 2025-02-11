using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace TradeAppWpf.Source
{
    public class ClientWebSocketApi : ITestConnectorWebSocket
    {
        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;
        public event Action<Candle> CandleSeriesProcessing;

        private ClientWebSocket _ws = new ClientWebSocket();
        private string _pair = "";

        public void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            SubscribeCandlesAsync(pair);
        }
        private async Task SubscribeCandlesAsync(string pair)
        {
            _pair = pair;
            await _ws.ConnectAsync(new Uri("wss://api-pub.bitfinex.com/ws/2"), CancellationToken.None);

            if (_ws.State != WebSocketState.Open)
            {
                return;
            };
            var subscriptionMessage = $@"
            {{
                ""event"": ""subscribe"",
                ""channel"": ""candles"",
                ""key"": ""trade:1m:{pair}""
            }}";
            var bytes = Encoding.UTF8.GetBytes(subscriptionMessage);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            _ = TradeHandler();
        }
        private async Task TradeHandler()
        {
            var buffer = new byte[1024];
            while (_ws.State == WebSocketState.Open)
            {
                var message = await _ws.ReceiveAsync(buffer, CancellationToken.None);
                if (message.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }
                string msg = Encoding.UTF8.GetString(buffer);

                CandleSeriesProcessing.Invoke(new Candle() { Pair = msg});

                // обработка
                try
                {
                    var candles = JsonSerializer.Deserialize<List<List<JsonElement>>>(msg);
                    List<Candle> candleList = new List<Candle>();
                    foreach (var currCandle in candles)
                    {
                        Candle newCandle = new Candle()
                        {
                            Pair = _pair,
                            OpenPrice = currCandle[1].GetInt64(),
                            HighPrice = currCandle[3].GetInt64(),
                            LowPrice = currCandle[4].GetInt64(),
                            ClosePrice = currCandle[2].GetInt64(),
                            TotalVolume = currCandle[5].GetDecimal(),
                            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(currCandle[0].GetInt64())
                        };
                        candleList.Add(newCandle);
                    }

                    CandleSeriesProcessing.Invoke(new Candle() { Pair = "Обработка сообщения закончена" });

                    foreach (var currCandle in candleList)
                        CandleSeriesProcessing.Invoke(currCandle);
                } catch
                {
                    CandleSeriesProcessing.Invoke(new Candle() { Pair = "Не удалось обработать сообщение" });
                }
            }

            CandleSeriesProcessing.Invoke(new Candle() { Pair = "Выход из цикла обработки сообщений" });
        }

        public void SubscribeTrades(string pair, int maxCount = 100)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeCandles(string pair)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeTrades(string pair)
        {
            throw new NotImplementedException();
        }
    }
}
