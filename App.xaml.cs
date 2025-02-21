using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using MSProj_Analog.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace MSProj_Analog
{
    public partial class App : Application
    {
        public string[] Parameters { get; set; }
        private readonly IHost _host;
        public App()
        {
            _host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAddResourceService, AddResourceService>();
            services.AddSingleton<IAddTaskService, AddTaskService>();
            services.AddSingleton<MainWindow>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Parameters = e.Args;
            base.OnStartup(e);
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();

            //var addResourceWindow = _host.Services.GetRequiredService<AddResourceWindow>();
            //addResourceWindow.Resources = new ObservableCollection<Resource>();
            using (var context = new AppDbContext())
            {
                var resultResourceList = context.Resources.Where(r => r.ProjectTaskId == null).ToList();
                mainWindow.Resources = new ObservableCollection<Resource>(resultResourceList);

                var resultTaskList = context.Tasks.Where(t => t.AssignedResource != null).ToList();
                mainWindow.Tasks = new ObservableCollection<ProjectTask>(resultTaskList);

                var resultFulltaskList = context.Tasks.Where(t => t.AssignedResource == null).ToList();
                mainWindow.FullTasks = new ObservableCollection<ProjectTask>(resultFulltaskList);
            }

            mainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }

    }

}
