using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using MSProj_Analog.Config;
using MSProj_Analog.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MSProj_Analog
{
    public partial class AddResourceWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Resource> Resources { get; set; }

        public AddResourceWindow(ObservableCollection<Resource> resources)
        {
            InitializeComponent();
            Resources = resources;
            DataContext = this;
        }

        private void OnSaveResourceClick(object sender, RoutedEventArgs e)
        {
            string name = ResourceNameTextBox.Text;
            decimal standardRate;
            decimal? overtimeRate = null;
            ResourceType resource = (ResourceType)int.Parse(ResourceTypeTextBox.Text);

            if (decimal.TryParse(StandardRateTextBox.Text, out standardRate))
            {
                if (decimal.TryParse(OvertimeRateTextBox.Text, out decimal tempOvertimeRate))
                {
                    overtimeRate = tempOvertimeRate;
                }

                var newResource = new Resource(resource, name, standardRate, overtimeRate);

                Resources.Add(newResource);
                using (var context = new AppDbContext())
                {
                    context.Resources.Add(newResource);
                    context.SaveChanges();
                }

            }
            else
            {
                MessageBox.Show(ConfigOptions.Messages.InvalidDataMessage);
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
