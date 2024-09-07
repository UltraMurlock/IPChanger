using System.ComponentModel;
using IPChanger.NetworkConfiguration;

namespace IPChanger
{
    internal class MainVM : BindableBase
    {
        public string[] AvailableInterfaces => _model.AvailableInterfaces;

        public string SelectedAdapterName 
        { 
            get
            {
                return _model.SelectedAdapterName;
            }
            set
            {
                _model.SelectAdapter(value);
            } 
        }

        public bool SelectedValidAdapter => _model.SelectedValidAdapter;

        public IpConfig ActualIpConfig
        {
            get { return _model.ActualIpConfig; }
        }

        public IpConfig IpConfig
        {
            get { return _model.GetPreviousConfig(SelectedAdapterName ?? string.Empty); }
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
                _model.SetConfig(SelectedAdapterName, IpConfig);
            });
        }



        private void ResendPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
