using System.Collections.ObjectModel;
using System.Linq;
using AutoBinance.Models;

namespace AutoBinance.ViewModels
{
    public class SymbolViewModel : SymbolModel
    {
        public SymbolViewModel(string? symbol, decimal? price) : base(symbol, price)
        {
        }

        public void AddOrder(OrderModel order)
        {
            Orders.Add(order);
            Orders = (ObservableCollection<OrderModel>)Orders.OrderByDescending(o => o.Time);
            RaisePropertyChangedEvent(nameof(Orders));
        }
    }
}
