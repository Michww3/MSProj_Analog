using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Enums;
using MSProj_Analog.Interfaces;
using MSProj_Analog.Services;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace MSProj_Analog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
            services.AddSingleton<MainWindow>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Parameters = e.Args;
            base.OnStartup(e);
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();

            mainWindow.Resources = new ObservableCollection<Resource>();

            mainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }

    }

}
