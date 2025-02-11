using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TradeAppWpf.Source;

namespace TradeAppWpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ClientWebSocketApi _clientWebSocketApi = new ClientWebSocketApi();
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Trade> Trades { get; private set; } = new();

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }
        public void ButtonTradeClickHandler()
        {
            _clientWebSocketApi.SubscribeTrades(InputText);
        }
        public void ButtonCandleClickHandler()
        {
            _clientWebSocketApi.SubscribeCandles(InputText, 30);
        }

        public MainViewModel()
        {
            _clientWebSocketApi.CandleSeriesProcessing += CandleHandler;
        }

        private void CandleHandler(Candle candle)
        {
            //LogText += candle.Pair + '\n';

            LogText += $"Пара: {candle.Pair}\n" +
               $"Время: {candle.OpenTime:yyyy-MM-dd HH:mm:ss}\n" +
               $"Цена открытия: {candle.OpenPrice}\n" +
               $"Макс. цена: {candle.HighPrice}\n" +
               $"Мин. цена: {candle.LowPrice}\n" +
               $"Цена закрытия: {candle.ClosePrice}\n" +
               $"Сумма сделок: {candle.TotalPrice}\n" +
               $"Общий объем: {candle.TotalVolume}\n" +
               $"-----------------------------\n";
        }

        private string _logText;
        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }
    }
}
