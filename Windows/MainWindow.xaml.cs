using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

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

        private void ViewDataButton_Click(object sender, RoutedEventArgs e)
        {
            var viewDataWindow = new ViewDataWindow();
            viewDataWindow.ShowDialog();
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

    }
}