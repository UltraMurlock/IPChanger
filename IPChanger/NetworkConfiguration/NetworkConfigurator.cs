using System.Diagnostics;

namespace IPChanger.NetworkConfiguration
{
    public class NetworkConfigurator
    {
        public static bool SetIp(string networkInterfaceName, string ipAddress, string subnetMask)
        {
            string args = $"interface ip set address \"{networkInterfaceName}\" static {ipAddress} {subnetMask}";

            return CallNetsh(args);
        }

        public static bool SetDhcp(string networkInterfaceName)
        {
            string args = $"interface ip set address \"{networkInterfaceName}\" dhcp";

            return CallNetsh(args);
        }



        private static bool CallNetsh(string args)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo("netsh");
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Verb = "runas";
            startInfo.Arguments = args;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            bool success = process.ExitCode == 0;
            process.Dispose();
            return success;
        }
    }
}
