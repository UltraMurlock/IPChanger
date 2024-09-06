using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using IPChanger.NetworkConfiguration;

namespace IPChanger
{
    internal class Model : BindableBase
    {
        public string InterfaceName = "Ethernet";
        public IpConfig ActualIpConfig;

        private const string _previousConfigPath = ".\\last-config.json";



        public Model()
        {
            ActualIpConfig = new IpConfig();
        }

        public void Initialize()
        {
            Task.Run(ActualUpdateLoopAsync);
            RaisePropertyChanged("InterfaceName");
        }



        public async Task ActualUpdateLoopAsync()
        {
            while(true)
            {
                UpdateActualIpConfig();
                await Task.Delay(1000);
            }
        }

        public void UpdateIpSettings(IpConfig ipConfig)
        {
            SaveIpConfig(ipConfig);

            if(ipConfig.DhcpEnabled && CheckDhcp() == ipConfig.DhcpEnabled)
                return;

            if(ipConfig.DhcpEnabled)
                NetworkConfigurator.SetDhcp(InterfaceName);
            else
                NetworkConfigurator.SetIp(InterfaceName, ipConfig.IpAddress, ipConfig.SubnetMask);

            UpdateActualIpConfig();
        }

        public IpConfig? LoadPreviousIpConfig()
        {
            if(!File.Exists(_previousConfigPath))
                return null;

            using(FileStream stream = new FileStream(_previousConfigPath, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<IpConfig>(stream);
        }


        private void UpdateActualIpConfig()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ActualIpConfig = GetActualIpConfig();
            RaisePropertyChanged("ActualIpConfig");
            sw.Stop();
        }

        private IpConfig GetActualIpConfig()
        {
            var networkInterface = GetNetworkInterface(InterfaceName);
            if(networkInterface == null)
                throw new Exception("Wrong interface name");

            IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
            IPv4InterfaceProperties ipv4Properites = ipProperties.GetIPv4Properties();

            UnicastIPAddressInformation? ipInfo = null;
            ipInfo = ipProperties.UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

            string ipAddress = ipInfo?.Address.ToString() ?? string.Empty;
            string subnetMask = ipInfo?.IPv4Mask.ToString() ?? string.Empty;

            return new IpConfig {
                DhcpEnabled = ipv4Properites.IsDhcpEnabled,
                IpAddress = ipAddress,
                SubnetMask = subnetMask
            };
        }

        private bool CheckDhcp()
        {
            var networkInterface = GetNetworkInterface(InterfaceName);
            IPv4InterfaceProperties ipv4Properites = networkInterface.GetIPProperties().GetIPv4Properties();
            return ipv4Properites.IsDhcpEnabled;
        }

        private NetworkInterface? GetNetworkInterface(string interfaceName)
        {
            foreach(NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if(networkInterface.Name == interfaceName)
                    return networkInterface;
            }

            return null;
        }

        private void SaveIpConfig(IpConfig ipConfig)
        {
            using(FileStream stream = new FileStream(_previousConfigPath, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize(stream, ipConfig);
        }
    }
}
