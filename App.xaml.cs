using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using MSProj_Analog.Services;
using MSProj_Analog.Windows;
using System.Collections.ObjectModel;
using System.Windows;

namespace MSProj_Analog
{
    public partial class App : Application
    {
        public string[] Parameters { get; set; }
        private readonly IHost _host;
        public static IServiceProvider Services => ((App)Current)._host.Services;
        public App()
        {
            _host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices).Build();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAddResourceService, AddResourceService>();
            services.AddSingleton<IAddTaskService, AddTaskService>();
            services.AddSingleton<IAddResourceToTaskService, AddResourceToTaskService>();
            services.AddSingleton<ChartWindow>();
            services.AddSingleton<AddTaskWindow>();
            services.AddSingleton<AddResourceWindow>();
            services.AddSingleton<AddTaskWindow>();
            services.AddSingleton<MainWindow>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Parameters = e.Args;
            base.OnStartup(e);
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            var addResourceWindow = _host.Services.GetRequiredService<AddResourceWindow>();
            var addTaskWindow = _host.Services.GetRequiredService<AddTaskWindow>();
            var addResourceToTaskWindow = _host.Services.GetRequiredService<AddResourceToTaskWindow>();

            using (var context = new AppDbContext())
            {
                var resultResourceList = context.Resources.Where(r => r.ProjectTaskId == null).ToList();
                mainWindow.Resources = new ObservableCollection<Resource>(resultResourceList);

                var resultTaskList = context.Tasks.Where(t => t.Resource == null).ToList();
                mainWindow.Tasks = new ObservableCollection<ProjectTask>(resultTaskList);

                var resultFulltaskList = context.Tasks.Where(t => t.Resource != null).ToList();
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
