using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace IPChanger.NetworkConfiguration
{
    public class NetworkAdapter
    {
        public string Name { get; private set; }



        public NetworkAdapter(string interfaceName)
        {
            if(GetNetworkInterface(interfaceName) == null)
                throw new Exception($"Не найден интерфейс {interfaceName}");

            Name = interfaceName;
        }



        public IpConfig GetActualConfig()
        {
            var networkInterface = GetNetworkInterface(Name);
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

        public void SetConfig(IpConfig config)
        {
            if(config.DhcpEnabled)
                NetworkConfigurator.SetDhcp(Name);
            else
                NetworkConfigurator.SetIp(Name, config.IpAddress, config.SubnetMask);
        }



        private NetworkInterface? GetNetworkInterface(string interfaceName)
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            for(int i = 0; i < interfaces.Length; i++)
            {
                if(interfaces[i].Name == interfaceName)
                    return interfaces[i];
            }
            return null;
        }
    }
}
