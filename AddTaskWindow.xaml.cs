using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace MSProj_Analog
{
    public partial class AddTaskWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<ProjectTask> Tasks { get; set; }

        public AddTaskWindow(ObservableCollection<ProjectTask> tasks)
        {
            InitializeComponent();
            Tasks = tasks;
            DataContext = this;
        }
        private void OnAddTaskClick(object sender, RoutedEventArgs e)
        {
            string name = TaskNameTextBox.Text;
            DateTime startDate = StartDateCalendar.DisplayDate;
            DateTime endDate = EndDateCalendar.DisplayDate;
            var task = new ProjectTask(name, startDate, endDate);

            Tasks.Add(task);

            using(var context  = new AppDbContext())
            {
                context.Tasks.Add(task);
                context.SaveChanges();  
            }
        }

        public void OnAddResourceToTaskClick(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var resourceId = Int32.Parse(ResourceIdTextBox.Text);
                var taskId = Int32.Parse(TaskIdTextBox.Text);

                var res = context.Resources.SingleOrDefault(r => r.Id == resourceId);
                var task = context.Tasks.SingleOrDefault(t => t.Id == taskId);

                if (res != null && task != null)
                {
                    res.ProjectTask = task;
                    task.AssignedResource.Add(res);
                    context.SaveChanges();
                }
                else
                {
                    MessageBox.Show(ConfigOptions.Messages.InvalidDataMessage);
                }
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
