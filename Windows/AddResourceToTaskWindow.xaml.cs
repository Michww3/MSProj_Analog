using Microsoft.Extensions.DependencyInjection;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MSProj_Analog
{

    public partial class AddResourceToTaskWindow : Window, INotifyPropertyChanged
    {
        IAddResourceToTaskService addResourceToTaskService = App.Services.GetRequiredService<IAddResourceToTaskService>();
        public ObservableCollection<ProjectTask> Tasks { get; set; }
        new public ObservableCollection<Resource> Resources { get; set; }

        public AddResourceToTaskWindow(ObservableCollection<Resource> resources, ObservableCollection<ProjectTask> tasks)
        {
            InitializeComponent();
            Resources = resources;
            Tasks = tasks;
            DataContext = this;
        }
        public void OnAddResourceToTaskClick(object sender, RoutedEventArgs e)
        {
            int resourceId;
            int taskId;

            try
            {
                resourceId = Int32.Parse(ResourceIdTextBox.Text);
                taskId = Int32.Parse(TaskIdTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show(ConfigOptions.Messages.InvalidData);
                return;
            }
            addResourceToTaskService.AddResourceToTask(new AppDbContext(), Tasks, Resources, resourceId, taskId);

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
