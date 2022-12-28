using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfClient.MessageBox;
using WpfClient.MVVM;

namespace WpfClient.ViewModels
{
    public class UserViewModel : ObservableObject
    {
        private bool _shownCredentailsMessage = false;

        public BinanceClient Client { get; set; }
        public BinanceSocketClient SocketClient { get; set; }
        public CancellationTokenSource MainCancelSource { get; set; }
        private readonly CancellationTokenSource UsdCancelSource;

        private readonly IMessageBoxService messageBoxService = new MessageBoxService();

        private bool isUsdStartStreamErrorMessageShowed;
        private bool isUsdSubscribeUserUpdatesErrorMessageShowed;

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

        private SymbolViewModel? selectedSymbol;
        public SymbolViewModel? SelectedSymbol
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

        private BotViewModel? selectedBot;
        public BotViewModel? SelectedBot
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

        private ObservableCollection<SymbolViewModel> allPrices;
        public ObservableCollection<SymbolViewModel> AllPrices
        {
            get { return allPrices; }
            set
            {
                allPrices = value;
                RaisePropertyChangedEvent(nameof(AllPrices));
            }
        }

        private ObservableCollection<BotViewModel> activeBots;
        public ObservableCollection<BotViewModel> ActiveBots
        {
            get { return activeBots; }
            set
            {
                activeBots = value;
                RaisePropertyChangedEvent(nameof(ActiveBots));
            }
        }        
        
        private ObservableCollection<BotViewModel> pausedBots;
        public ObservableCollection<BotViewModel> PausedBots
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

        public UserViewModel(string Username, string ApiKey, string ApiSecret)
        {
            username = Username;
            apiKey = ApiKey;
            apiSecret = ApiSecret;
            Client = new();
            SocketClient = new();
            allPrices = new ObservableCollection<SymbolViewModel>();
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

            InitalizeClients();
            Task.Run(() => SubscribeUserUpdates());


            MainCancelSource = new CancellationTokenSource();
            UsdCancelSource = new CancellationTokenSource();

            isUsdStartStreamErrorMessageShowed = false;
            isUsdSubscribeUserUpdatesErrorMessageShowed = false;

            activeBots = new ObservableCollection<BotViewModel>();
            pausedBots = new ObservableCollection<BotViewModel>();

        }

        public void AddLog(string log)
        {
            Application.Current.Dispatcher.BeginInvoke(delegate ()
            {
                Logs.Add(log);
            });
        }

        public void ChangeSymbol()
        {
            if (SelectedSymbol != null)
            {
                Task.Run(async () => await Task.WhenAll(GetOpenOrders(SelectedSymbol), Get24HourStats(SelectedSymbol)));
            }
        }

        public async Task GetAllSymbols()
        {
            // Get USDT & Coins prices
            var resultUsd = await Client.UsdFuturesApi.ExchangeData.GetPricesAsync();
            
            if (resultUsd.Success)
            {
                AllPrices = new ObservableCollection<SymbolViewModel>( resultUsd.Data.Select(r => new SymbolViewModel(r.Symbol, r.Price)).OrderBy(x => x.Symbol) );
            }
            else
            {
                messageBoxService.ShowMessage($"Binance veri akışında hata : {(resultUsd.Error == null ? "NULL" : resultUsd.Error.Message)}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var subscribeResult = await SocketClient.UsdFuturesStreams.SubscribeToAllTickerUpdatesAsync(data =>
            {
                foreach (var ud in data.Data)
                {
                    var symbol = AllPrices.SingleOrDefault(p => p.Symbol == ud.Symbol);
                    if (symbol != null)
                        symbol.Price = ud.LastPrice;
                }
            }, MainCancelSource.Token);

            if (!subscribeResult.Success)
                messageBoxService.ShowMessage($"Binance fiyat güncellemelerine katılmada hata (Usd Futures) : {subscribeResult.Error}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public async Task GetOpenOrders(SymbolViewModel? symbol)
        {
            if (symbol == null)
                return;

            if (string.IsNullOrEmpty(apiKey))
            {
                if (!_shownCredentailsMessage)
                {
                    _shownCredentailsMessage = true;
                    messageBoxService.ShowMessage($"Lütfen Api ayarlarınızı giriniz ve doğru girdiğinizden emin olunuz!", "Api Girilmemiş", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return;
            }

            var resultUsd = await Client.UsdFuturesApi.Trading.GetOpenOrdersAsync(symbol.Symbol);
            if (resultUsd.Success)
            {
                symbol.Orders = new ObservableCollection<OrderViewModel>(resultUsd.Data.OrderByDescending(d => d.CreateTime).Select(o => new OrderViewModel()
                {
                    Id = o.Id,
                    Time = o.CreateTime,
                    Type = o.Type,
                    Price = o.Quantity,
                    Filled = o.LastFilledQuantity,
                    ReduceOnly = o.ReduceOnly,
                    PositionSide = o.PositionSide,
                    StopPrice = o.StopPrice ?? 0,
                }));
            }
            else
            {
                messageBoxService.ShowMessage($"Binance veri akışında hata : {(resultUsd.Error == null ? "NULL" : resultUsd.Error.Message)}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public async Task Get24HourStats(SymbolViewModel symbol)
        {
            var resultUsd = await Client.UsdFuturesApi.ExchangeData.GetTickerAsync(symbol.Symbol);
            if (resultUsd.Success)
            {
                symbol.HighPrice = resultUsd.Data.HighPrice;
                symbol.LowPrice = resultUsd.Data.LowPrice;
                symbol.Volume = resultUsd.Data.QuoteVolume;
                symbol.PriceChangePercent = resultUsd.Data.PriceChangePercent;
            }
            else
            {
                messageBoxService.ShowMessage($"Binance veri akışında hata : {(resultUsd.Error == null ? "NULL" : resultUsd.Error.Message)}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void InitalizeClients()
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                Client = new BinanceClient();
                SocketClient = new BinanceSocketClient();
            }
            else
            {
                Client = new BinanceClient(new BinanceClientOptions() { ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(apiKey, apiSecret) });
                SocketClient = new BinanceSocketClient(new BinanceSocketClientOptions() { ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials(apiKey, apiSecret) });
            }
        }

        public BotViewModel GetBot()
        {
            if (ActiveBots.Any(x => x.Symbol.Symbol.Equals(selectedSymbol?.Symbol))) 
                return ActiveBots.First(x => x.Symbol.Symbol.Equals(selectedSymbol?.Symbol));

            if (PausedBots.Any(x => x.Symbol.Symbol.Equals(selectedSymbol?.Symbol))) 
                return PausedBots.First(x => x.Symbol.Symbol.Equals(selectedSymbol?.Symbol));

            return new BotViewModel(selectedSymbol ?? new SymbolViewModel(null, null));
        }

        public async Task StartBot(BotViewModel bot)
        {
            if(PausedBots.Any(x => x.Equals(bot)))
            {
                PausedBots.Remove(bot);
                await GetOpenOrders(bot.Symbol);

                if (bot.Symbol.Orders.Any(x => x.Id.Equals(bot.LastOpenOrderId))) return;

                bot.IsEnabled = true;
                ActiveBots.Add(bot);

                SelectedBot = bot;
                
                AddLog($"{bot.Symbol.Symbol} botu tekrar başlatıldı.");

                return;
            }
            
            if (!ActiveBots.Any(x => x.Equals(bot)))
            {
                // Open first open order from stop market
                OrderSide side;
                decimal? quan;
                decimal? stopPrice;

                if(bot.FirstOrderType == PositionSide.Long)
                {
                    side = OrderSide.Sell;
                    quan = bot.SizeShort / bot.StopPriceShort;
                    stopPrice = bot.StopPriceShort;
                }
                else
                {
                    side = OrderSide.Buy;
                    quan = bot.SizeLong / bot.StopPriceLong;
                    stopPrice = bot.StopPriceLong;
                }

                var resultOpenOrder = await Client.UsdFuturesApi.Trading.PlaceOrderAsync(
                                        bot.Symbol.Symbol, side,
                                        FuturesOrderType.StopMarket,
                                        quan, stopPrice: stopPrice,
                                        workingType: WorkingType.Mark,
                                        positionSide: bot.LastOpenOrderPositionSide);

                if (resultOpenOrder.Success)
                {
                    bot.LastOpenOrderId = resultOpenOrder.Data.Id;
                    bot.FirstReverseOrderId = resultOpenOrder.Data.Id;
                    bot.AddLog($"Başlangıç açık emiri başarıyla verildi. Id : {resultOpenOrder.Data.Id}");
                }
                else
                {
                    messageBoxService.ShowMessage($"Başlangıç açık emri verilemedi. Hata : {(resultOpenOrder.Error == null ? "NULL" : resultOpenOrder.Error.Message)}", "Hata !", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Open first order from market
                var resultOrder = await Client.UsdFuturesApi.Trading.PlaceOrderAsync(
                                        bot.Symbol.Symbol,
                                        bot.FirstOrderType == PositionSide.Long ? OrderSide.Buy : OrderSide.Sell,
                                        FuturesOrderType.Market,
                                        decimal.Round(bot.FirstOrderSize ?? 0 / bot.Symbol.Price, 1),
                                        positionSide: bot.FirstOrderType);

                if (resultOrder.Success)
                {
                    bot.FirstOrderId = resultOrder.Data.Id;
                    bot.AddLog($"Başlangıç işlemi başarıyla açıldı. Id : {resultOrder.Data.Id}");
                }
                else
                {
                    messageBoxService.ShowMessage($"Başlangıç işlemi açılamadı. Hata : {(resultOrder.Error == null ? "NULL" : resultOrder.Error.Message)}", "Hata !", MessageBoxButton.OK, MessageBoxImage.Error);
                    var cancelResult = await Client.CoinFuturesApi.Trading.CancelOrderAsync(bot.Symbol.Symbol, bot.LastOpenOrderId);
                    if (!cancelResult.Success)
                    {
                        messageBoxService.ShowMessage("İlk açık emir iptal edilemedi. Lütfen manuel deneyiniz.", "Hata !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }

                PausedBots.Remove(bot);
                bot.IsEnabled = true;
                ActiveBots.Add(bot);

                AddLog($"{bot.Symbol.Symbol} botu başlatıldı.");
            }
        }

        public void PauseBot(BotViewModel bot)
        {
            ActiveBots.Remove(bot);
            bot.IsEnabled = false;
            PausedBots.Add(bot);

            AddLog($"{bot.Symbol.Symbol} botu durduruldu.");

            SelectedBot = bot;
        }
        
        public Task ResetBot(BotViewModel bot)
        {
            if(ActiveBots.Any(x => x.Symbol.Symbol.Equals(bot.Symbol.Symbol)))
            {
                ActiveBots.Remove(bot);
                AddLog($"{bot.Symbol.Symbol} botu durduruldu.");
            }
            if (PausedBots.Any(x => x.Symbol.Symbol.Equals(bot?.Symbol.Symbol)))
            {
                PausedBots.Remove(bot);
            }

            SelectedBot = new BotViewModel(SelectedSymbol);

            return Task.CompletedTask;
        }

        private async Task PlaceReverseOpenOrderAsync(BotViewModel bot)
        {
            var price = bot.LastOpenOrderPositionSide == PositionSide.Long ? (bot.SizeShort ?? 0)/ bot.StopPriceShort
                                                                           : (bot.SizeLong ?? 0)/ bot.StopPriceLong;
            var result = await Client.UsdFuturesApi.Trading.PlaceOrderAsync(
                                                                    bot.Symbol.Symbol,
                                                                    bot.LastOpenOrderPositionSide == PositionSide.Short ? OrderSide.Buy : OrderSide.Sell,
                                                                    FuturesOrderType.StopMarket,
                                                                    price,
                                                                    workingType: WorkingType.Mark,
                                                                    positionSide: bot.LastOpenOrderPositionSide == PositionSide.Short ? PositionSide.Long : PositionSide.Short,
                                                                    stopPrice: bot.LastOpenOrderPositionSide == PositionSide.Long ? bot.StopPriceShort : bot.StopPriceLong);
            if (result.Success)
            {
                bot.LastOpenOrderId = result.Data.Id;
                bot.LastOpenOrderPositionSide = bot.LastOpenOrderPositionSide == PositionSide.Short ? PositionSide.Long : PositionSide.Short;
                bot.ExpiredOrderUpdated = false;
                bot.AddLog($"Yeni açık emir başarıyla verildi. Id : {result.Data.Id}");
            }
            else
            {
                bot.ExpiredOrderUpdated = false;
                string errorStr = $"Yeni açık emir verilemedi. Hata : {(result.Error == null ? "NULL" : result.Error.Message)}";
                if(result.Error?.Code == 2021)// -2021 ORDER_WOULD_IMMEDIATELY_TRIGGER
                {
                    errorStr += "\nParametreleri kontrol ediniz!";
                }else if (result.Error?.Code == 2019)//-2019 MARGIN_NOT_SUFFICIEN
                {
                    errorStr += "\nMarjin yeterli değil!";
                }
                bot.AddLog(errorStr);
            }
        }

        public async Task SubscribeUserUpdates()
        {
            var startUserStream = await Client.UsdFuturesApi.Account.StartUserStreamAsync(UsdCancelSource.Token);
            if (!startUserStream.Success)
            {
                if (!isUsdStartStreamErrorMessageShowed)
                {
                    string _ = $"Hesap akışına kayıt olmada hata : {(startUserStream.Error == null ? "NULL" : startUserStream.Error.Message)}\n30 saniye sonra tekrar denenecektir. (Lütfen ağ bağlantınızı kontrol ediniz.)";
                    messageBoxService.ShowMessage(_, "Hata !", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddLog(_);
                    isUsdStartStreamErrorMessageShowed = true;
                }

                UsdCancelSource.Cancel();
                await Task.Delay(30000);
                UsdCancelSource.TryReset();

                await Task.Run(async () => await SubscribeUserUpdates());

                return;
            }
            else
            {
                ListenKey = startUserStream.Data;
                isUsdStartStreamErrorMessageShowed = false;
            }

            var subscribeUserUpdates = await SocketClient.UsdFuturesStreams.SubscribeToUserDataUpdatesAsync(ListenKey, null, null, null, 
            async data =>  //Bot =>
            {
                var orderUpdate = data.Data.UpdateData;
                foreach (var bot in ActiveBots)
                {
                    if (orderUpdate.OrderId == bot.LastOpenOrderId)
                    {
                        // Open open order with reverse lastOpenOrderSide
                        if (orderUpdate.Status == OrderStatus.Expired)
                        {
                            bot.ExpiredOrderUpdated = true;
                        }
                        else if (orderUpdate.Status == OrderStatus.New && bot.ExpiredOrderUpdated)
                        {
                            await PlaceReverseOpenOrderAsync(bot);
                        }
                    }
                }
               await GetOpenOrders(SelectedSymbol);
            }, 
            async data =>
            {
                //When listenKeyExpired
                UsdCancelSource.Cancel();
                await Task.Delay(1);
                UsdCancelSource.TryReset();

                await Task.Run(async () => await SubscribeUserUpdates());

                AddLog($"Binance bağlantısı yenileniyor. Log : {data.Data.Event}");
            }, ct: UsdCancelSource.Token);

            if (!subscribeUserUpdates.Success)
            {
                if (!isUsdSubscribeUserUpdatesErrorMessageShowed)
                {
                    messageBoxService.ShowMessage($"Hesap akışına katılmada hata : {(subscribeUserUpdates.Error == null ? "NULL" : subscribeUserUpdates.Error.Message)}\n30 saniye sonra tekrar denenecektir.\n\n(Akış tekrar sağlanana kadar 30 saniyede bir tekrar denenecektir.\nLütfen ağ bağlantınızı kontrol ediniz.)", "Hata !", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddLog($"Hesap akışına katılmada hata : {(subscribeUserUpdates.Error == null ? "NULL" : subscribeUserUpdates.Error.Message)}\n30 saniye sonra tekrar denenecektir. (Lütfen ağ bağlantınızı kontrol ediniz.)");
                    isUsdSubscribeUserUpdatesErrorMessageShowed = true;
                }

                UsdCancelSource.Cancel();
                await Task.Delay(30000);
                UsdCancelSource.TryReset();

                await Task.Run(async () => await SubscribeUserUpdates());
            }
            else isUsdSubscribeUserUpdatesErrorMessageShowed = false;
        }
    }
}