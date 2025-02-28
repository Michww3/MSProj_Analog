using Microsoft.EntityFrameworkCore;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.AvalonDock.Properties;

namespace MSProj_Analog
{

    public partial class AddResourceToTaskWindow : Window, INotifyPropertyChanged
    {
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
            var resourceId = Int32.Parse(ResourceIdTextBox.Text);
            var taskId = Int32.Parse(TaskIdTextBox.Text);

            var res = Resources.FirstOrDefault(r => r.Id == resourceId);
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            using (var context = new AppDbContext())
            {
                //var res = context.Resources.SingleOrDefault(r => r.Id == resourceId);
                //var task = context.Tasks.SingleOrDefault(t => t.Id == taskId);

                //var rs = context.Resources
                //    .Where(r => r.Id == resourceId)
                //    .ExecuteUpdate(r => r
                //    .SetProperty(r => r.ProjectTask,
                //    context.Tasks.SingleOrDefault(t => t.Id == taskId)));

                //var ts = context.Tasks
                //    .Where(t => t.Id == taskId)
                //    .ExecuteUpdate(t => t
                //    .SetProperty(t => t.Resource,
                //    context.Resources.SingleOrDefault(r => r.Id == resourceId)));

                var dbRes = context.Resources.SingleOrDefault(r => r.Id == resourceId);
                var dbTask = context.Tasks.SingleOrDefault(t => t.Id == taskId);

                //if (res != null && task != null)
                //{
                //    res.ProjectTask = task;
                //    task.Resource = res;
                //    context.Resources.Update(res);
                //    context.Tasks.Update(task);
                //    context.SaveChanges();
                //    Tasks.Remove(task);
                //    Resources.Remove(res);
                //}
                if (dbRes != null && dbTask != null)
                {
                    dbRes.ProjectTask = dbTask;
                    dbTask.Resource = dbRes;
                    context.SaveChanges();
                    Tasks.Remove(task);
                    Resources.Remove(res);
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
