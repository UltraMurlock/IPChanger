using System.Net.NetworkInformation;
using IPChanger.NetworkConfiguration;
using Prism.Mvvm;

namespace IPChanger
{
    internal class Model : BindableBase
    {
        public IpConfig ActualIpConfig;

        public string[] AvailableInterfaces => GetAvailableInterfaceNames();

        public bool SelectedValidAdapter => _adapter != null;
        public string SelectedAdapterName => _adapter?.Name ?? string.Empty;
        private NetworkAdapter? _adapter;

        private const string _settingsPath = ".\\settings.json";
        private Settings _settings;



        public Model()
        {
            ActualIpConfig = new IpConfig();

            _settings = Settings.Load(_settingsPath);
            SelectAdapter(_settings.SelectedAdapter);
        }

        public void Initialize()
        {
            Task.Run(UpdateActualIpAsync);
            Task.Run(ActualIpUpdateLoopAsync);
            RaisePropertyChanged(nameof(AvailableInterfaces));
            RaisePropertyChanged(nameof(SelectedValidAdapter));
            RaisePropertyChanged(nameof(SelectedAdapterName));
        }

        public async Task UpdateActualIpAsync()
        {
            if(_adapter == null)
                return;

            IpConfig ipConfig;
            while(true)
            {
                ipConfig = _adapter.GetActualConfig();
                if(ipConfig.IpAddress != string.Empty)
                    break;

                await Task.Delay(100);
            }

            ActualIpConfig = ipConfig;
            RaisePropertyChanged(nameof(ActualIpConfig));
        }

        public async Task ActualIpUpdateLoopAsync()
        {
            while(true)
            {
                await Task.Delay(5000);

                if(_adapter != null)
                {
                    ActualIpConfig = _adapter.GetActualConfig();
                    RaisePropertyChanged("ActualIpConfig");
                }
            }
        }



        public void SelectAdapter(string interfaceName)
        {
            if(interfaceName == string.Empty)
                return;

            _settings.SelectedAdapter = interfaceName;
            _settings.Save(_settingsPath);

            _adapter = new NetworkAdapter(interfaceName);
            RaisePropertyChanged(nameof(SelectedValidAdapter));
            RaisePropertyChanged("IpConfig");

            Task.Run(UpdateActualIpAsync);
        }



        public void SetConfig(string interfaceName, IpConfig ipConfig)
        {
            if(_adapter == null)
                return;

            _settings.SetConfig(ipConfig, interfaceName);
            _settings.Save(_settingsPath);

            _adapter.SetConfig(ipConfig);

            Task.Run(UpdateActualIpAsync);
        }

        public IpConfig GetPreviousConfig(string interfaceName)
        {
            return _settings.GetConfig(interfaceName);
        }



        private string[] GetAvailableInterfaceNames()
        {
            List<string> availables = new List<string>();
            foreach(NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var interfaceType = networkInterface.NetworkInterfaceType;
                if(interfaceType == NetworkInterfaceType.Ethernet || interfaceType == NetworkInterfaceType.Wireless80211)
                    availables.Add(networkInterface.Name);
            }
            return availables.ToArray();
        }
    }
}
