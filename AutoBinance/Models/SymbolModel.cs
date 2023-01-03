using System.Collections.ObjectModel;
using AutoBinance.MVVM;

namespace AutoBinance.Models
{
    public class SymbolModel : ObservableObject
    {
        private string? symbol;
        public string Symbol
        {
            get { return symbol ?? ""; }
            set
            {
                symbol = value;
                RaisePropertyChangedEvent(nameof(Symbol));
            }
        }

        private decimal? price;
        public decimal Price
        {
            get { return price ?? 0; }
            set
            {
                price = value;
                RaisePropertyChangedEvent(nameof(Price));
                RaisePropertyChangedEvent(nameof(BotModel.ProfitTextLong));
                RaisePropertyChangedEvent(nameof(BotModel.ProfitTextShort));
            }
        }

        private decimal priceChangePercent;
        public decimal PriceChangePercent
        {
            get { return priceChangePercent; }
            set
            {
                priceChangePercent = value;
                RaisePropertyChangedEvent(nameof(PriceChangePercent));
            }
        }

        private decimal highPrice;
        public decimal HighPrice
        {
            get { return highPrice; }
            set
            {
                highPrice = value;
                RaisePropertyChangedEvent(nameof(HighPrice));
            }
        }

        private decimal lowPrice;
        public decimal LowPrice
        {
            get { return lowPrice; }
            set
            {
                lowPrice = value;
                RaisePropertyChangedEvent(nameof(LowPrice));
            }
        }

        private decimal volume;
        public decimal Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                RaisePropertyChangedEvent(nameof(Volume));
            }
        }

        private ObservableCollection<OrderModel> orders;
        public ObservableCollection<OrderModel> Orders
        {
            get { return orders; }
            set
            {
                orders = value;
                RaisePropertyChangedEvent(nameof(Orders));
            }
        }

        public SymbolModel(string? symbol, decimal? price)
        {
            this.symbol = symbol;
            this.price = price;
            orders = new ObservableCollection<OrderModel>();
        }
    }
}
