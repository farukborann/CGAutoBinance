using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using System.Collections.ObjectModel;
using System.Linq;
using AutoBinance.MVVM;

namespace AutoBinance.Models
{
    public abstract class UserModel : ObservableObject
    {
        internal bool _shownCredentailsMessage = false;

        public BinanceClient Client { get; set; }
        public BinanceSocketClient SocketClient { get; set; }

        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                RaisePropertyChangedEvent(nameof(Username));
            }
        }

        private string apiKey;
        public string ApiKey
        {
            get { return apiKey; }
            set
            {
                apiKey = value;
                RaisePropertyChangedEvent(nameof(ApiKey));
            }
        }

        private string apiSecret;
        public string ApiSecret
        {
            get { return apiSecret; }
            set
            {
                apiSecret = value;
                RaisePropertyChangedEvent(nameof(ApiSecret));
            }
        }        
        
        public string ListenKey {  get; set; }

        public bool SymbolIsSelected => SelectedSymbol != null;
        public bool IsCredentialed => !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(apiSecret) && SymbolIsSelected;

        private SymbolModel? selectedSymbol;
        public SymbolModel? SelectedSymbol
        {
            get { return selectedSymbol; }
            set
            {
                selectedSymbol = value;
                SelectedBot = GetBot();
                RaisePropertyChangedEvent(nameof(SymbolIsSelected));
                RaisePropertyChangedEvent(nameof(IsCredentialed));
                RaisePropertyChangedEvent(nameof(SelectedBot));
                RaisePropertyChangedEvent(nameof(SelectedSymbol));
                ChangeSymbol();
            }
        }

        private BotModel? selectedBot;
        public BotModel? SelectedBot
        {
            get
            {
                return selectedBot;
            }
            set
            {
                selectedBot = value;
                RaisePropertyChangedEvent(nameof(SelectedBot));
            }
        }

        private ObservableCollection<SymbolModel> allPrices;
        public ObservableCollection<SymbolModel> AllPrices
        {
            get { return allPrices; }
            set
            {
                allPrices = value;
                RaisePropertyChangedEvent(nameof(AllPrices));
            }
        }

        private ObservableCollection<BotModel> activeBots;
        public ObservableCollection<BotModel> ActiveBots
        {
            get { return activeBots; }
            set
            {
                activeBots = value;
                RaisePropertyChangedEvent(nameof(ActiveBots));
            }
        }        
        
        private ObservableCollection<BotModel> pausedBots;
        public ObservableCollection<BotModel> PausedBots
        {
            get { return pausedBots; }
            set
            {
                pausedBots = value;
                RaisePropertyChangedEvent(nameof(PausedBots));
            }
        }

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

        public UserModel(string Username, string ApiKey, string ApiSecret)
        {
            username = Username;
            apiKey = ApiKey;
            apiSecret = ApiSecret;
            Client = new();
            SocketClient = new();
            allPrices = new ObservableCollection<SymbolModel>();
            logs = new ObservableCollection<string>();
            ListenKey = "";

            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                UsdFuturesApiOptions = new BinanceApiClientOptions()
                {
                    TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
                },
                CoinFuturesApiOptions = new BinanceApiClientOptions()
                {
                    TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,
                }
            });

            Logs = new ObservableCollection<string>();

            activeBots = new ObservableCollection<BotModel>();
            pausedBots = new ObservableCollection<BotModel>();

        }

        public BotModel GetBot()
        {
            if (ActiveBots.Any(x => x.Symbol.Symbol.Equals(SelectedSymbol?.Symbol)))
                return ActiveBots.First(x => x.Symbol.Symbol.Equals(SelectedSymbol?.Symbol));

            if (PausedBots.Any(x => x.Symbol.Symbol.Equals(SelectedSymbol?.Symbol)))
                return PausedBots.First(x => x.Symbol.Symbol.Equals(SelectedSymbol?.Symbol));

            return new BotModel(SelectedSymbol ?? new SymbolModel(null, null));
        }

        public abstract void ChangeSymbol();
    }
}