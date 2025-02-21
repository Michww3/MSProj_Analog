using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
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
            using(var context = new AppDbContext())
            {
                var resultList = context.Resources.Where(r => r.ProjectTaskId == null).ToList<Resource>();
                Resources = new ObservableCollection<Resource>(resultList);
            }
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
    }
}
