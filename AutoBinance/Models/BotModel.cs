using Binance.Net.Enums;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AutoBinance.MVVM;

namespace AutoBinance.Models
{
    public class BotModel : ObservableObject
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

        private SymbolModel symbol;
        public SymbolModel Symbol
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
        private decimal? stopPriceLong;
        public decimal? StopPriceLong
        {
            get { return stopPriceLong; }
            set
            {
                stopPriceLong = value;
                RaisePropertyChangedEvent(nameof(StopPriceLong));
            }
        }

        private decimal? sizeLong;
        public decimal? SizeLong
        {
            get { return sizeLong; }
            set
            {
                sizeLong = value;
                RaisePropertyChangedEvent(nameof(SizeLong));
                RaisePropertyChangedEvent(nameof(ProfitTextLong));
            }
        }

        //Lower
        private decimal? stopPriceShort;
        public decimal? StopPriceShort
        {
            get { return stopPriceShort; }
            set
            {
                stopPriceShort = value;
                RaisePropertyChangedEvent(nameof(StopPriceShort));
            }
        }

        //Size Change
        private decimal? sizeChange;
        public decimal? SizeChange
        {
            get { return sizeChange; }
            set
            {
                sizeChange = value;
                RaisePropertyChangedEvent(nameof(SizeChange));
            }
        }

        private decimal? sizeShort;
        public decimal? SizeShort
        {
            get { return sizeShort; }
            set
            {
                sizeShort = value;
                RaisePropertyChangedEvent(nameof(SizeShort));
                RaisePropertyChangedEvent(nameof(ProfitTextShort));
            }
        }

        //Calc Profit
        private decimal? coinLeverageLong;
        public decimal? CoinLeverageLong
        {
            get { return coinLeverageLong; }
            set
            {
                coinLeverageLong = value;
                RaisePropertyChangedEvent(nameof(coinLeverageLong));
                RaisePropertyChangedEvent(nameof(ProfitTextLong));
            }
        }

        private decimal? profitUSDTLong;
        public decimal? ProfitUSDTLong
        {
            get { return profitUSDTLong; }
            set
            {
                profitUSDTLong = value;
                RaisePropertyChangedEvent(nameof(profitUSDTLong));
                RaisePropertyChangedEvent(nameof(ProfitTextLong));
            }
        }

        public string? ProfitTextLong
        {
            get
            {
                decimal? profitPrice = symbol.Price * ((profitUSDTLong / SizeLong * 100 / coinLeverageLong) + 1);
                return $"USDT Kar/Zarar İçin {symbol.Symbol} Fiyatı {(profitPrice == null ? 0 : decimal.Round((decimal)profitPrice, 8)).ToString(new CultureInfo("en-US", false))} USDT Olmalıdır";
            }
        }

        private decimal? coinLeverageShort;
        public decimal? CoinLeverageShort
        {
            get { return coinLeverageShort; }
            set
            {
                coinLeverageShort = value;
                RaisePropertyChangedEvent(nameof(coinLeverageShort));
                RaisePropertyChangedEvent(nameof(ProfitTextShort));
            }
        }

        private decimal? profitUSDTShort;
        public decimal? ProfitUSDTShort
        {
            get { return profitUSDTShort; }
            set
            {
                profitUSDTShort = value;
                RaisePropertyChangedEvent(nameof(profitUSDTShort));
                RaisePropertyChangedEvent(nameof(ProfitTextShort));
            }
        }

        public string? ProfitTextShort
        {
            get
            {
                decimal? profitPrice = symbol.Price * ((profitUSDTShort / SizeShort * 100 / coinLeverageShort) + 1);
                return $"USDT Kar/Zarar İçin {symbol.Symbol} Fiyatı {(profitPrice == null ? 0 : decimal.Round((decimal)profitPrice, 8)).ToString(new CultureInfo("en-US", false))} USDT Olmalıdır";
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

        public long FirstOrderId { get; set; }

        public long FirstReverseOrderId { get; set; }

        public long LastOpenOrderId { get; set; }

        public PositionSide LastOpenOrderPositionSide { get; set; }

        public bool ExpiredOrderUpdated { get; set; }

        public BotModel(SymbolModel? symbol)
        {
            logs = new ObservableCollection<string>();

            isEnabled = false;
            this.symbol = symbol ?? new SymbolModel(null, null);

            ISideChangeCommand = new DelegateCommand((o) => ChangeSide());
            firstOrderType = PositionSide.Long;

            LastOpenOrderId = 0;
            FirstOrderId = 0;
            FirstReverseOrderId = 0;
            LastOpenOrderPositionSide = FirstOrderType == PositionSide.Long ? PositionSide.Short : PositionSide.Long;

            ExpiredOrderUpdated = false;

            Logs = new ObservableCollection<string>();
        }
    }
}
