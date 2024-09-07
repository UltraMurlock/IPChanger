namespace IPChanger.NetworkConfiguration
{
    public class IpConfig
    {
        public bool DhcpEnabled { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }



        public IpConfig()
        {
            DhcpEnabled = false;
            IpAddress = string.Empty;
            SubnetMask = string.Empty;
        }



        public static IpConfig Default
        {
            get
            {
                return new IpConfig() {
                    DhcpEnabled = false,
                    IpAddress = "192.168.0.0",
                    SubnetMask = "255.255.255.0"
                };
            }
        }
    }
}
