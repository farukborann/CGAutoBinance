using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfClient.MVVM;

namespace WpfClient.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        #region NetworkTest
        private bool connectivity;
        private long pingTime;

        public string ConnectivityString
        {
            get
            {
                if (connectivity && pingTime <= 500) return "Stabil Bağlantı";
                else if (connectivity && pingTime <= 1000) return "Yavaş Bağlantı";
                else if (connectivity && pingTime > 1000) return "Aşırı Yavaş Bağlantı";
                else if (!connectivity) return "Bağlantı Yok";
                else return "Bağlantı durumu bilinmiyor!";
            }
        }
        public Brush? ConnectivityColor
        {
            get
            {
                if (connectivity)
                {
                    if (pingTime <= 500)
                    {
                        return (SolidColorBrush?)new BrushConverter().ConvertFromString("#0DDB76");
                    }
                    else
                    {
                        return (SolidColorBrush?)new BrushConverter().ConvertFromString("#FFD966");
                    }
                }
                else
                {
                    return (SolidColorBrush?)new BrushConverter().ConvertFromString("#DB374C");
                }
            }
        }

        private void TestConnectivity(object? sender, EventArgs e)
        {
            Task.Run(() => TestConnectivityAsync());
        }

        public Task TestConnectivityAsync()
        {
            try
            {
                Ping myPing = new();
                PingReply reply = myPing.Send("www.binance.com", 1000);
                if (reply != null)
                {
                    connectivity = true;
                    RaisePropertyChangedEvent(nameof(ConnectivityString));
                    RaisePropertyChangedEvent(nameof(ConnectivityColor));
                    pingTime = reply.RoundtripTime;

                }
            }
            catch
            {
                connectivity = false;
                RaisePropertyChangedEvent(nameof(ConnectivityString));
                RaisePropertyChangedEvent(nameof(ConnectivityColor));
            }

            return Task.CompletedTask;
        }
        #endregion

        public ICommand IBotCommand { get; set; }
        public ICommand IResetBotCommand { get; set; }

        public ICommand IAddUserCommand { get; set; }
        public ICommand IDeleteUserCommand { get; set; }
        public ICommand IUpdateUserCommand { get; set; }

        private ObservableCollection<UserViewModel> users;
        public ObservableCollection<UserViewModel> Users
        {
            get { return users; }
            set
            {
                users = value;
                RaisePropertyChangedEvent(nameof(Users));
            }
        }

        private UserViewModel? currentUser;
        public UserViewModel CurrentUser
        {
            get { return currentUser ?? new UserViewModel("default", "",""); }
            set
            {
                currentUser = value;
                RaisePropertyChangedEvent(nameof(CurrentUser.IsCredentialed));
                RaisePropertyChangedEvent(nameof(CurrentUser));
                CurrentUser.ChangeSymbol();
                Task.Run(() => CurrentUser.GetAllSymbols());
            }
        }

        public MainViewModel()
        {
            connectivity = false;
            pingTime = 0;

            DispatcherTimer TestConnectivityTimer = new();
            TestConnectivityTimer.Interval = TimeSpan.FromMinutes(1);
            TestConnectivityTimer.Tick += TestConnectivity;
            TestConnectivityTimer.Start();

            DispatcherTimer KeepUserStreamAliveTimer = new();
            KeepUserStreamAliveTimer.Interval = TimeSpan.FromMinutes(10);
            KeepUserStreamAliveTimer.Tick += KeepUsersStreamAlive;
            KeepUserStreamAliveTimer.Start();

            users = new ObservableCollection<UserViewModel>();

            IBotCommand = new DelegateCommand(async (o) => await BotCommand(CurrentUser.SelectedBot));
            IResetBotCommand = new DelegateCommand(async (o) => await CurrentUser.ResetBot(CurrentUser.SelectedBot));

            IAddUserCommand = new DelegateCommand(async (o) => await AddUser());
            IDeleteUserCommand = new DelegateCommand(async (o) => await DeleteUser(CurrentUser));
            IUpdateUserCommand = new DelegateCommand(async (o) => await UpdateUser());

            if (File.Exists("Config/config.json"))
            {
                using StreamReader file = new("Config/config.json");
                JsonSerializer.Deserialize<List<Tuple<string, string, string>>>(file.ReadToEnd())?.ForEach(x => Users.Add(new UserViewModel(x.Item1, x.Item2, x.Item3)));
                RaisePropertyChangedEvent(nameof(Users));
                file.Close();
            }

            if (users.Count > 0) CurrentUser = users[0];
            else { currentUser = new UserViewModel("default", "", ""); }
            Task.Run(() => CurrentUser.GetAllSymbols());
        }

        public async Task BotCommand(BotViewModel bot)
        {
            if (!CurrentUser.ActiveBots.Any(x => x.Symbol.Symbol == bot?.Symbol.Symbol))
            {
                await CurrentUser.StartBot(bot);
            }
            else
            {
                CurrentUser.PauseBot(bot);
            }
        }

        public Task AddUser()
        {
            Users.Add(new UserViewModel("default", "", ""));
            RaisePropertyChangedEvent(nameof(Users));

            using (StreamWriter file = new("Config/config.json"))
            {
                List<Tuple<string, string, string>> us = new();
                foreach (var user in Users)
                {
                    us.Add(Tuple.Create(user.Username, user.ApiKey, user.ApiSecret));
                }
                string asd = JsonSerializer.Serialize(us);
                file.Write(asd);
            }
            return Task.CompletedTask;
        }

        public Task DeleteUser(UserViewModel delUser)
        {
            users.Remove(delUser);
            RaisePropertyChangedEvent(nameof(Users));

            using (StreamWriter file = new("Config/config.json"))
            {
                List<Tuple<string, string, string>> us = new();
                foreach (var user in users)
                {
                    us.Add(Tuple.Create(user.Username, user.ApiKey, user.ApiSecret));
                }
                string asd = JsonSerializer.Serialize(us);
                file.Write(asd);
            }
            return Task.CompletedTask;
        }

        public Task UpdateUser()
        {
            RaisePropertyChangedEvent(nameof(Users));
            currentUser?.InitalizeClients();
            using (StreamWriter file = new("Config/config.json"))
            {
                List<Tuple<string, string, string>> us = new();
                foreach (var user in users)
                {
                    us.Add(Tuple.Create(user.Username, user.ApiKey, user.ApiSecret));
                }
                string asd = JsonSerializer.Serialize(us);
                file.Write(asd);
            }
            return Task.CompletedTask;
        }

        private void KeepUsersStreamAlive(object? sender, EventArgs e)
        {
            Task.Run(() => KeepUsersStreamAliveAsync());
        }

        private async Task KeepUsersStreamAliveAsync()
        {
            foreach (UserViewModel user in Users.ToList())
            {
                if (user.Client != null && !string.IsNullOrEmpty(user.ListenKey) && user.ActiveBots.Count > 0)
                {
                    var ret = await user.Client.UsdFuturesApi.Account.KeepAliveUserStreamAsync(user.ListenKey);
                    if (ret.Success)
                    {
                        user.AddLog($"Dinleme anahtarı canlı tutuldu. ListenKey : {user.ListenKey}");
                    }
                    else
                    {
                        user.AddLog($"Dinleme anahtarı canlı tutulamadı. Hata : {(ret.Error == null ? "NULL" : ret.Error.Message)} \nListenKey : {(ret.Data ?? "-")}");
                    }
                }
            }
        }
    }
}
