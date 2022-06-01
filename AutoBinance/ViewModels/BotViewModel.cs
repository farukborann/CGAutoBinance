using Binance.Net.Enums;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfClient.MVVM;

namespace WpfClient.ViewModels
{
    public class BotViewModel : ObservableObject
    {
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                RaisePropertyChangedEvent(nameof(IsEnabled));
                RaisePropertyChangedEvent(nameof(IsEnabledString));
            }
        }

        public string IsEnabledString
        {
            get
            {
                if (IsEnabled) return "Durdur";
                else return "Başlat";
            }
        }

        private SymbolViewModel symbol;
        public SymbolViewModel Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
                RaisePropertyChangedEvent(nameof(Symbol));
            }
        }

        //FirstOrderButton
        public ICommand ISideChangeCommand { get; set; }
        public void ChangeSide()
        {
            if (FirstOrderType == PositionSide.Long) FirstOrderType = PositionSide.Short;
            else if (FirstOrderType == PositionSide.Short) FirstOrderType = PositionSide.Long;
        }

        public string FirstOrderTypeString => firstOrderType == PositionSide.Long ? "Long" : "Short";
        public Brush? FirstOrderTypeColor => firstOrderType == PositionSide.Long ? (SolidColorBrush?)new BrushConverter().ConvertFromString("#0DDB76") : (SolidColorBrush?)new BrushConverter().ConvertFromString("#DB374C");

        //FirstOrder
        private PositionSide firstOrderType;
        public PositionSide FirstOrderType
        {
            get { return firstOrderType; }
            set
            {
                firstOrderType = value;
                LastOpenOrderPositionSide = firstOrderType == PositionSide.Long ? PositionSide.Short : PositionSide.Long;
                RaisePropertyChangedEvent(nameof(FirstOrderType));
                RaisePropertyChangedEvent(nameof(FirstOrderTypeString));
                RaisePropertyChangedEvent(nameof(FirstOrderTypeColor));
            }
        }

        private decimal? firstOrderSize;
        public decimal? FirstOrderSize
        {
            get { return firstOrderSize; }
            set
            {
                firstOrderSize = value;
                RaisePropertyChangedEvent(nameof(FirstOrderSize));
            }
        }

        //Upper
        private decimal? stopPriceUpper;
        public decimal? StopPriceUpper
        {
            get { return stopPriceUpper; }
            set
            {
                stopPriceUpper = value;
                RaisePropertyChangedEvent(nameof(StopPriceUpper));
            }
        }

        private decimal? sizeUpper;
        public decimal? SizeUpper
        {
            get { return sizeUpper; }
            set
            {
                sizeUpper = value;
                RaisePropertyChangedEvent(nameof(SizeUpper));
            }
        }

        //Lower
        private decimal? stopPriceLower;
        public decimal? StopPriceLower
        {
            get { return stopPriceLower; }
            set
            {
                stopPriceLower = value;
                RaisePropertyChangedEvent(nameof(StopPriceLower));
            }
        }

        private decimal? sizeLower;
        public decimal? SizeLower
        {
            get { return sizeLower; }
            set
            {
                sizeLower = value;
                RaisePropertyChangedEvent(nameof(SizeLower));
            }
        }

        //Logs
        private ObservableCollection<string> logs;
        public ObservableCollection<string> Logs
        {
            get { return logs; }
            set
            {
                logs = value;
                RaisePropertyChangedEvent(nameof(Logs));
            }
        }

        public void AddLog(string log)
        {
            Application.Current.Dispatcher.BeginInvoke(delegate ()
            {
                Logs.Add(log);
            });
        }

        public long LastOrderId { get; set; }

        public long LastOpenOrderId { get; set; }

        public PositionSide LastOpenOrderPositionSide { get; set; }

        public bool ExpiredOrderUpdated { get; set; }

        public BotViewModel(SymbolViewModel? symbol)
        {
            logs = new ObservableCollection<string>();

            isEnabled = false;
            this.symbol = symbol ?? new SymbolViewModel(null, null);

            ISideChangeCommand = new DelegateCommand((o) => ChangeSide());
            firstOrderType = PositionSide.Long;

            LastOpenOrderId = 0;
            LastOrderId = 0;
            LastOpenOrderPositionSide = FirstOrderType == PositionSide.Long ? PositionSide.Short : PositionSide.Long;

            ExpiredOrderUpdated = false;

            Logs = new ObservableCollection<string>();
        }
    }
}
