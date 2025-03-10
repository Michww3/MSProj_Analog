using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Windows;
using MSProj_Analog.Config;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Markup;

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

        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var path = ConfigOptions.Path;
            ExportToXml<Resource>(Resources, $"{path}XmlExportResources.xml");
            ExportToXml<ProjectTask>(Tasks, $"{path}XmlExportTasks.xml");
            ExportToXml<ProjectTask>(FullTasks, $"{path}XmlExportFullTasks.xml");
        }

        public static void ExportToXml<T>(ICollection<T> collection, string filePath)
        {
            try
            {
                // Создаем сериализатор
                XmlSerializer serializer = new XmlSerializer(typeof(List<T>));

                // Открываем файл для записи
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Сериализуем коллекцию в файл
                    serializer.Serialize(writer, new List<T>(collection));
                }

                MessageBox.Show("Данные успешно экспортированы в XML!");
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }
    }
}
