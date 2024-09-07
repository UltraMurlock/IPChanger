using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using IPChanger.NetworkConfiguration;
using Prism.Mvvm;
using Prism.Commands;

namespace IPChanger
{
    internal class MainVM : BindableBase
    {
        public string[] AvailableInterfaces => _model.AvailableInterfaces;

        public string SelectedAdapterName 
        { 
            get => _model.SelectedAdapterName;
            set => _model.SelectAdapter(value);
        }

        public bool SelectedValidAdapter => _model.SelectedValidAdapter;

        public IpConfig ActualIpConfig => _model.ActualIpConfig;
        public IpConfig IpConfig => _model.GetPreviousConfig(SelectedAdapterName ?? string.Empty);

        public DelegateCommand<IpConfig> SetCommand { get; }


        private readonly Model _model;



        public MainVM()
        {
            _model = new Model();
            _model.PropertyChanged += ResendPropertyChanged;
            _model.Initialize();

            SetCommand = new DelegateCommand<IpConfig>(IpConfig => {

                string pattern = "^((25[0-5]|(2[0-4]|1\\d|[1-9]|)\\d)\\.?\\b){4}$";

                if(!Regex.IsMatch(IpConfig.IpAddress, pattern))
                {
                    MessageBox.Show("Неверно введёт IP адрес");
                    return;
                }

                if(!Regex.IsMatch(IpConfig.SubnetMask, pattern))
                {
                    MessageBox.Show("Неверно введена маска подсети");
                    return;
                }

                _model.SetConfig(SelectedAdapterName, IpConfig);
            });
        }



        private void ResendPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
