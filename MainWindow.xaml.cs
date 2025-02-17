using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MSProj_Analog
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ProjectTask> _tasks;
        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; OnPropertyChanged("Tasks"); }
        }

        private ObservableCollection<Resource> _resources;
        new public ObservableCollection<Resource> Resources
        {
            get { return _resources; }
            set { _resources = value; OnPropertyChanged("Resources"); }
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
    }
}
