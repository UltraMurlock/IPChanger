using System.ComponentModel;
using IPChanger.NetworkConfiguration;

namespace IPChanger
{
    internal class MainVM : BindableBase
    {
        public string InterfaceName => _model.InterfaceName;

        public bool ActualDhcpEnabled => _model.ActualIpConfig.DhcpEnabled;
        public string ActualIpAddress => _model.ActualIpConfig.IpAddress;
        public string ActualSubnetMask => _model.ActualIpConfig.SubnetMask;

        public IpConfig ActualIpConfig
        {
            get { return _model.ActualIpConfig; }
        }

        public IpConfig IpConfig
        {
            get { return _model.LoadPreviousIpConfig(); }
        }

        public DelegateCommand<IpConfig> SetCommand { get; }

        private readonly Model _model;



        public MainVM()
        {
            _model = new Model();
            _model.PropertyChanged += ResendPropertyChanged;
            _model.Initialize();

            SetCommand = new DelegateCommand<IpConfig>(IpConfig => {
                //Тут нужна проверка на валидность ввода
                _model.UpdateIpSettings(IpConfig);
            });
        }



        private void ResendPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
