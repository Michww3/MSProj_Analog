using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace MSProj_Analog
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Resource> _resources;
        public new ObservableCollection<Resource> Resources
        {
            get { return _resources; }
            set { _resources = value; OnPropertyChanged("Resources"); }
        }

        public ObservableCollection<ProjectTask> _tasks;
        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; OnPropertyChanged("Tasks"); }
        }

        private ObservableCollection<ProjectTask> _fullTasks;
        public ObservableCollection<ProjectTask> FullTasks
        {
            get { return _fullTasks; }
            set { _fullTasks = value; OnPropertyChanged("FullTasks"); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAddResourceClick(object sender, RoutedEventArgs e)
        {
            var addResourceWindow = new AddResourceWindow(Resources);
            addResourceWindow.ShowDialog();
        }

        private void OnAddTaskClick(object sender, RoutedEventArgs e)
        {
            var addTaskWindow = new AddTaskWindow(Tasks);
            addTaskWindow.ShowDialog();
        }

        private void OnAddResourceToTaskClick(object sender, RoutedEventArgs e)
        {
            var addResourceToTaskWindow = new AddResourceToTaskWindow(Resources, Tasks);
            addResourceToTaskWindow.ShowDialog();
            RefreshFullTasks();
        }

        private void RefreshFullTasks()
        {
            using (var context = new AppDbContext())
            {
                var updatedTasks = context.Tasks.Where(t => t.Resource != null).ToList();
                FullTasks = new ObservableCollection<ProjectTask>(updatedTasks);
            }
        }

        private void OnCreateGanttChartClick(object sender, RoutedEventArgs e)
        {
            var chartWindow = new GanttChartWindow(FullTasks);
            chartWindow.ShowDialog();
        }

        private void OnCreatePieChartClick(object sender, RoutedEventArgs e)
        {
            var pieChartWindow = new PieChartWindow(FullTasks);
            pieChartWindow.ShowDialog();
        }

        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            Test(FullTasks);
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {

            ExportAllToXml(Resources, Tasks, FullTasks, ConfigOptions.Path + "Export\\", $"DataExport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml");
        }

 
        public static void ExportAllToXml(IEnumerable<Resource> resources,IEnumerable<ProjectTask> tasks,IEnumerable<ProjectTask> fullTasks,string directoryPath,string fileName = "CombinedExport.xml")
        {
            
                var xml = new XElement("ExportData",
                    new XElement("Resources",
                        from resource in resources
                        select new XElement("Resource",
                            new XAttribute("Id", resource.Id),
                            new XAttribute("Name", resource.Name),
                            new XAttribute("StandardRate", resource.StandardRate),
                            resource.OvertimeRate.HasValue ? new XAttribute("OvertimeRate", resource.OvertimeRate.Value) : null,
                            new XAttribute("Type", resource.Type.ToString())
                        )
                    ),
                    new XElement("Tasks",
                        from task in tasks
                        select new XElement("Task",
                            new XAttribute("Id", task.Id),
                            new XAttribute("Name", task.Name),
                            new XAttribute("StartDate", task.StartDate.ToString("yyyy-MM-dd")),
                            new XAttribute("EndDate", task.EndDate.ToString("yyyy-MM-dd"))
                        )
                    ),
                    new XElement("FullTasks",
                        from task in fullTasks
                        select new XElement("Task",
                            new XAttribute("Id", task.Id),
                            new XAttribute("Name", task.Name),
                            new XAttribute("StartDate", task.StartDate.ToString("yyyy-MM-dd")),
                            new XAttribute("EndDate", task.EndDate.ToString("yyyy-MM-dd")),
                            new XAttribute("AssignedResourceId", task.ResourceId ?? throw new ArgumentNullException(nameof(task.ResourceId)))
                        )
                    )
                );

            xml.Save(Path.Combine(directoryPath, fileName));
        }

        public void Test(ICollection<ProjectTask> fullTask)
        {
            foreach (var task in fullTask)
            {
                if (task.Resource != null) // Проверяем, есть ли связанный ресурс
                {
                    MessageBox.Show($"Задача: {task.Name}, Ставка ресурса: {task.Resource.StandardRate}");
                }
                else
                {
                    MessageBox.Show($"Задача: {task.Name} не имеет связанного ресурса.");
                }
            }
        }

    }
}