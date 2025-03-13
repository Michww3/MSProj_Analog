using Microsoft.Extensions.DependencyInjection;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.AvalonDock.Properties;

namespace MSProj_Analog
{
    public partial class AddResourceWindow: Window, INotifyPropertyChanged
    {
        IAddResourceService addResourceService = App.Services.GetRequiredService<IAddResourceService>();

        private ObservableCollection<Resource> _resources;
        public new ObservableCollection<Resource> Resources
        {
            get { return _resources; }
            set { _resources = value; OnPropertyChanged("Resources"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddResourceWindow(ObservableCollection<Resource> resources)
        {
            InitializeComponent();
            Resources = resources;
            DataContext = this;
        }


        private void OnAddResourceClick(object sender, RoutedEventArgs e)
        {

            string name = ResourceNameTextBox.Text;
            decimal standardRate = Decimal.Parse(StandardRateTextBox.Text);
            decimal? overtimeRate = Decimal.Parse(OvertimeRateTextBox.Text);
            var selectedItem = ResourceTypeComboBox.SelectedValue as string;
            ResourceType resource;
            if (selectedItem != null)
            {
                resource = (ResourceType)Enum.Parse(typeof(ResourceType), selectedItem);
            }
            else
            {
                MessageBox.Show("Please select a valid resource type.");
                return;
            }


            var newResource = new Resource(resource, name, standardRate, overtimeRate);

            addResourceService.AddResource(new AppDbContext(),Resources,newResource);

        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }


    }
}
