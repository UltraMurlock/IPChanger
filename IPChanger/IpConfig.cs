namespace IPChanger
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
    }
}
