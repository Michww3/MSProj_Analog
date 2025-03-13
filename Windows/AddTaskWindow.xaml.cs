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
    public partial class AddTaskWindow : Window, INotifyPropertyChanged
    {
        IAddTaskService addTaskService = App.Services.GetRequiredService<IAddTaskService>();

        private ObservableCollection<ProjectTask> _tasks;

        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; OnPropertyChanged("Tasks"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AddTaskWindow(ObservableCollection<ProjectTask> tasks)
        {
            InitializeComponent();
            Tasks = tasks;
            DataContext = this;
        }

        private void OnAddTaskClick(object sender, RoutedEventArgs e)
        {
            string name = TaskNameTextBox.Text;
            DateTime startDate = StartDatePicker.DisplayDate;
            DateTime endDate = EndDatePicker.DisplayDate;
            if (startDate > endDate)
            {
                MessageBox.Show(ConfigOptions.Messages.InvalidData);
            }
            else
            {
                var task = new ProjectTask(name, startDate, endDate);
                addTaskService.AddTask(new AppDbContext(), Tasks, task);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
