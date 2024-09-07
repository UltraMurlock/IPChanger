using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using IPChanger.NetworkConfiguration;

namespace IPChanger
{
    internal class Model : BindableBase
    {
        public IpConfig ActualIpConfig;

        public string[] AvailableInterfaces => GetAvailableInterfaceNames();

        private NetworkAdapter? _adapter;

        private const string _previousConfigPath = ".\\last-config.json";



        public Model()
        {
            ActualIpConfig = new IpConfig();
        }

        public void Initialize()
        {
            Task.Run(ActualUpdateLoopAsync);
            RaisePropertyChanged(nameof(AvailableInterfaces));
        }

        public async Task ActualUpdateLoopAsync()
        {
            while(true)
            {
                if(_adapter != null)
                {
                    ActualIpConfig = _adapter.GetActualConfig();
                    RaisePropertyChanged("ActualIpConfig");
                }

                await Task.Delay(100);
            }
        }



        public void SelectAdapter(string interfaceName)
        {
            if(interfaceName == string.Empty)
                return;

            _adapter = new NetworkAdapter(interfaceName);
        }

        public void SetConfig(IpConfig ipConfig)
        {
            SaveIpConfig(ipConfig);
            _adapter?.SetConfig(ipConfig);
        }

        public IpConfig LoadPreviousIpConfig()
        {
            if(!File.Exists(_previousConfigPath))
                return IpConfig.Default;

            using(FileStream stream = new FileStream(_previousConfigPath, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<IpConfig>(stream) ?? IpConfig.Default;
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

        private void SaveIpConfig(IpConfig ipConfig)
        {
            using(FileStream stream = new FileStream(_previousConfigPath, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize(stream, ipConfig);
        }
    }
}
