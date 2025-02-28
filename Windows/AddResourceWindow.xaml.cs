using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using MSProj_Analog.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MSProj_Analog
{
    public partial class AddResourceWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Resource> _resources;
        public new ObservableCollection<Resource> Resources
        {
            get { return _resources; }
            set { _resources = value; OnPropertyChanged("Resources"); }
        }

        public AddResourceWindow(ObservableCollection<Resource> resources)
        {
            InitializeComponent();
            Resources = resources;
            DataContext = this;
        }
        public AddResourceWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnAddResourceClick(object sender, RoutedEventArgs e)
        {
            string name = ResourceNameTextBox.Text;
            decimal standardRate = Decimal.Parse(StandardRateTextBox.Text);
            decimal? overtimeRate = Decimal.Parse(OvertimeRateTextBox.Text);
            if (!int.TryParse(ResourceTypeTextBox.Text, out int typeValue) || !Enum.IsDefined(typeof(ResourceType), typeValue))
            {
                MessageBox.Show(ConfigOptions.Messages.InvalidResourceType, ConfigOptions.Messages.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ResourceType resource = (ResourceType)typeValue;


            var newResource = new Resource(resource, name, standardRate, overtimeRate);

            Resources.Add(newResource);
            using (var context = new AppDbContext())
            {
                context.Resources.Add(newResource);
                context.SaveChanges();
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
