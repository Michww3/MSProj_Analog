using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSProj_Analog.Interfaces;
using MSProj_Analog.Services;
using System.Windows;

namespace MSProj_Analog
{
    public partial class App : Application
    {
        public string[]? Parameters { get; set; }
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
            services.AddSingleton<IInitializeResourceService, InitializeResourceService>();
            services.AddSingleton<MainWindow>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Parameters = e.Args;
            base.OnStartup(e);
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            var initializeMainWindowResourceService = _host.Services.GetRequiredService<IInitializeResourceService>();
            initializeMainWindowResourceService.InitializeResource(mainWindow);
            mainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }

    }

}
