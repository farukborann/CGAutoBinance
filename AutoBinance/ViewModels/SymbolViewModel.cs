using System.Collections.ObjectModel;
using System.Linq;
using WpfClient.MVVM;

namespace WpfClient.ViewModels
{
    public class SymbolViewModel : ObservableObject
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

        private ObservableCollection<OrderViewModel> orders;
        public ObservableCollection<OrderViewModel> Orders
        {
            get { return orders; }
            set
            {
                orders = value;
                RaisePropertyChangedEvent(nameof(Orders));
            }
        }

        public SymbolViewModel(string? symbol, decimal? price)
        {
            this.symbol = symbol;
            this.price = price;
            orders = new ObservableCollection<OrderViewModel>();
        }

        public void AddOrder(OrderViewModel order)
        {
            Orders.Add(order);
            Orders = (ObservableCollection<OrderViewModel>)Orders.OrderByDescending(o => o.Time);
            RaisePropertyChangedEvent(nameof(Orders));
        }
    }
}
