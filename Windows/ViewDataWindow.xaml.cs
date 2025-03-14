using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace MSProj_Analog.Windows
{

    public partial class ViewDataWindow : Window
    {
        IInitializeResourceService initializeMainWindowResourceService = App.Services.GetRequiredService<IInitializeResourceService>();
        MainWindow mainWindow = App.Services.GetRequiredService<MainWindow>();

        private ObservableCollection<ProjectTask> _tasks;
        public ObservableCollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; }
        }
        private ObservableCollection<Resource> resources;
        new public ObservableCollection<Resource> Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewDataWindow()
        {
            InitializeComponent();
            using (var context = new AppDbContext())
            {
                Tasks = new ObservableCollection<ProjectTask>(context.Tasks.ToList<ProjectTask>());
                Resources = new ObservableCollection<Resource>(context.Resources.ToList<Resource>());
            }
            DataContext = this;

        }

        public static void ExportAllToXml(ICollection<Resource> resources, ICollection<ProjectTask> tasks, string directoryPath, string fileName = "CombinedExport.xml")
        {
            var xml = new XElement("ExportData",
                new XElement("Resources",
                    from resource in resources
                    select new XElement("Resource",
                        new XAttribute("Id", resource.Id),
                        new XAttribute("Name", resource.Name),
                        new XAttribute("StandardRate", resource.StandardRate),
                        resource.OvertimeRate.HasValue ? new XAttribute("OvertimeRate", resource.OvertimeRate.Value) : null,
                        new XAttribute("Type", resource.Type.ToString()),
                        resource.ProjectTaskId.HasValue ? new XAttribute("ProjectTaskId", resource.ProjectTaskId.Value) : null
                    )
                ),
                new XElement("Tasks",
                    from task in tasks
                    select new XElement("Task",
                        new XAttribute("Id", task.Id),
                        new XAttribute("Name", task.Name),
                        new XAttribute("StartDate", task.StartDate.ToString("yyyy-MM-dd")),
                        new XAttribute("EndDate", task.EndDate.ToString("yyyy-MM-dd")),
                        task.ResourceId.HasValue ? new XAttribute("ResourceId", task.ResourceId.Value) : null
                    )
                )
            );

            xml.Save(Path.Combine(directoryPath, fileName));
        }

        public void ImportDataFromXml(string xmlPath, AppDbContext context)
        {
            var xmlDoc = XDocument.Load(xmlPath);

            var resources = xmlDoc.Descendants("Resource").Select(x => new Resource
            {
                Id = (int)x.Attribute("Id"),
                Name = (string)x.Attribute("Name"),
                StandardRate = (decimal)x.Attribute("StandardRate"),
                OvertimeRate = (decimal?)x.Attribute("OvertimeRate"),
                Type = Enum.Parse<ResourceType>((string)x.Attribute("Type")),
            }).ToList();

            var tasks = xmlDoc.Descendants("Task").Select(x => new ProjectTask
            {
                Id = (int)x.Attribute("Id"),
                Name = (string)x.Attribute("Name"),
                StartDate = (DateTime)x.Attribute("StartDate"),
                EndDate = (DateTime)x.Attribute("EndDate"),
                ResourceId = (int?)x.Attribute("ResourceId")
            }).ToList();

            //expilicit insertion id for add resource to task
            using var transaction = context.Database.BeginTransaction(); // for check correct explicit insertion of Id

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Resources ON");
            context.Resources.AddRange(resources);
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Resources OFF");

            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Tasks ON");
            context.Tasks.AddRange(tasks);
            context.SaveChanges();
            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Tasks OFF");

            transaction.Commit();

            var tasksWithResources = context.Tasks.Where(t => t.ResourceId != null).ToList(); // Загружаем все задачи в память(Entity have 1 DataReader)

            foreach (var task in tasksWithResources)
            {
                var resource = context.Resources.SingleOrDefault(r => r.Id == task.ResourceId);
                resource.ProjectTask = task;
                context.SaveChanges();
            }

            Resources.Clear();
            foreach (var resource in resources)
            {
                Resources.Add(resource);
            }

            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
            initializeMainWindowResourceService.InitializeResource(mainWindow);
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Выберите место для сохранения",
                Filter = "XML файлы (*.xml)|*.xml",
                DefaultExt = "xml",
                FileName = "ExportData.xml",
                InitialDirectory = ConfigOptions.Path + "Export\\"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportAllToXml(Resources, Tasks, Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));
                MessageBox.Show("Данные успешно экспортированы!", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите XML-файл для импорта",
                Filter = "XML файлы (*.xml)|*.xml",
                DefaultExt = "xml",
                InitialDirectory = ConfigOptions.Path + "Export\\"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (var context = new AppDbContext())
                {
                    ImportDataFromXml(openFileDialog.FileName, context);
                }

                MessageBox.Show("Импорт завершен!", "Импорт данных", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}