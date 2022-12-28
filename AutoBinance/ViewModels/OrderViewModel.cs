using Binance.Net.Enums;
using System;
using WpfClient.MVVM;

namespace WpfClient.ViewModels
{
    public class OrderViewModel : ObservableObject
    {
        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChangedEvent(nameof(Id));
            }
        }

        private DateTime time;
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
                RaisePropertyChangedEvent(nameof(Time));
            }
        }

        private FuturesOrderType type;
        public FuturesOrderType Type
        {
            get { return type; }
            set
            {
                type = value;
                RaisePropertyChangedEvent(nameof(Type));
            }
        }

        private PositionSide positionSide;
        public PositionSide PositionSide
        {
            get { return positionSide; }
            set
            {
                positionSide = value;
                RaisePropertyChangedEvent(nameof(PositionSide));
            }
        }

        private decimal price;
        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                RaisePropertyChangedEvent(nameof(Price));
            }
        }

        private decimal stopPrice;
        public decimal StopPrice
        {
            get { return stopPrice; }
            set
            {
                stopPrice = value;
                RaisePropertyChangedEvent(nameof(StopPrice));
            }
        }

        private decimal filled;
        public decimal Filled
        {
            get { return filled; }
            set
            {
                filled = value;
                RaisePropertyChangedEvent(nameof(Filled));
            }
        }

        private bool reduceOnly;
        public bool ReduceOnly
        {
            get { return reduceOnly; }
            set
            {
                reduceOnly = value;
                RaisePropertyChangedEvent(nameof(ReduceOnly));
            }
        }
    }
}
