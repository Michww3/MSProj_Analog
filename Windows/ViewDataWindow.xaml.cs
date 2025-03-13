using Microsoft.Extensions.DependencyInjection;
using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Enums;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace MSProj_Analog.Windows
{

    public partial class ViewDataWindow : Window
    {
        IInitializeResourceService initializeMainWindowResourceService = App.Services.GetRequiredService<IInitializeResourceService>();
        MainWindow mainWindow = App.Services.GetRequiredService<MainWindow>();

        private ICollection<ProjectTask> _tasks;
        public ICollection<ProjectTask> Tasks
        {
            get { return _tasks; }
            set { _tasks = value; }
        }
        private ICollection<Resource> resources;
        new public ICollection<Resource> Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public ViewDataWindow()
        {
            InitializeComponent();
            using (var context = new AppDbContext())
            {
                Tasks = context.Tasks.ToList<ProjectTask>();
                Resources = context.Resources.ToList<Resource>();
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
                Name = (string)x.Attribute("Name"),
                StandardRate = (decimal)x.Attribute("StandardRate"),
                OvertimeRate = (decimal?)x.Attribute("OvertimeRate"),
                Type = Enum.Parse<ResourceType>((string)x.Attribute("Type")),
                ProjectTaskId = (int?)x.Attribute("AppointedTaskId")
            }).ToList();

            var tasks = xmlDoc.Descendants("Task").Select(x => new ProjectTask
            {
                Name = (string)x.Attribute("Name"),
                StartDate = (DateTime)x.Attribute("StartDate"),
                EndDate = (DateTime)x.Attribute("EndDate"),
                ResourceId = (int?)x.Attribute("ResourceId")
            }).ToList();

            context.Resources.AddRange(resources);
            context.Tasks.AddRange(tasks);
            context.SaveChanges();
            initializeMainWindowResourceService.InitializeResource(mainWindow);
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            ExportAllToXml(Resources, Tasks, ConfigOptions.Path+"Export\\");
        }

        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            ImportDataFromXml(ConfigOptions.Path + "Export\\" + "CombinedExport.xml", new AppDbContext());
        }

    }
}