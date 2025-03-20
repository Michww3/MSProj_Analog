using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;
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

        public static void ExportDataToXml(ICollection<Resource> resources, ICollection<ProjectTask> tasks, string directoryPath, string fileName = "CombinedExport.xml")
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

            var tasksWithResources = context.Tasks.Where(t => t.ResourceId != null).ToList(); // Загружаем все задачи в память(EntityFW have 1 DataReader)

            foreach (var task in tasksWithResources)
            {
                var resource = context.Resources.SingleOrDefault(r => r.Id == task.ResourceId);
                if (resource == null)
                    throw new NullReferenceException($"Resource with id: {task.ResourceId} not found");
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

            ConfigOptions.MessageBoxInfo("Импорт завершен!", "Импорт");
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = ConfigOptions.Export("Выберите место для сохранения", "XML файлы (*.xml)|*.xml", "xml", "ExportData.xml", ConfigOptions.Path + "Export\\");

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportDataToXml(Resources, Tasks, Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));
                ConfigOptions.MessageBoxExport("Xml экспорт");
            }
        }

        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = ConfigOptions.Import("Выберите XML-файл для импорта", "XML файлы (*.xml)|*.xml", "xml", ConfigOptions.Path + "Export\\");

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var context = new AppDbContext())
                    {
                        ImportDataFromXml(openFileDialog.FileName, context);
                    }
                }
                catch (Exception ex) when (ex is NullReferenceException || ex is InvalidOperationException || ex is ArgumentNullException)
                {
                    ConfigOptions.MessageBoxError("Data in xml file are corrupted", "Corrupted xml");
                }
                catch (DbUpdateException)
                {
                    ConfigOptions.MessageBoxError("Db already have data with the same id or xml file are corrupted", "Db update error");
                }
                catch(XmlException ex)
                {
                    ConfigOptions.MessageBoxError("Xml file error: " + ex.Message, "File error");
                }
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}