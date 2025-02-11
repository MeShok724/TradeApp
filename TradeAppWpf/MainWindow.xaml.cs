using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TradeAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel => (MainViewModel)DataContext;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonTrade_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ButtonTradeClickHandler();
        }

        private void ButtonCandle_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ButtonCandleClickHandler();
        }
    }
}