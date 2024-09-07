using System.IO;
using System.Text.Json;
using IPChanger.NetworkConfiguration;

namespace IPChanger
{
    [Serializable]
    public class Settings
    {
        public string SelectedAdapter { get; set; }
        public Dictionary<string, IpConfig> Configs { get; set; }

        private JsonSerializerOptions _serializerOptions;



        public Settings() 
        { 
            SelectedAdapter = string.Empty;
            Configs = new Dictionary<string, IpConfig>();

            _serializerOptions = new JsonSerializerOptions {
                WriteIndented = true
            };
        }



        public void SetSelectedAdapter(string interfaceName)
        {
            SelectedAdapter = interfaceName;
        }

        public void SetConfig(IpConfig config, string interfaceName)
        {
            if(Configs.ContainsKey(interfaceName))
                Configs[interfaceName] = config;
            else
                Configs.Add(interfaceName, config);
        }

        public IpConfig GetConfig(string interfaceName)
        {
            if(Configs.TryGetValue(interfaceName, out var config))
                return config;
            else
            {
                IpConfig newConfig = IpConfig.Default;
                Configs.Add(interfaceName, newConfig);
                return newConfig;
            }
        }



        public  static Settings Load(string path)
        {
            if(!File.Exists(path))
                return new Settings();

            using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return JsonSerializer.Deserialize<Settings>(stream) ?? new Settings();
        }

        public void Save(string path)
        {
            using(FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                JsonSerializer.Serialize(stream, this, _serializerOptions);
        }
    }
}
