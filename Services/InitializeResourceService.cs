using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using System.Collections.ObjectModel;

namespace MSProj_Analog.Services
{
    class InitializeResourceService : IInitializeResourceService
    {
        public void InitializeResource(MainWindow mainWindow)
        {
            using (var context = new AppDbContext())
            {
                var resultResourceList = context.Resources.Where(r => r.ProjectTaskId == null).ToList();
                mainWindow.Resources = new ObservableCollection<Resource>(resultResourceList);

                var resultTaskList = context.Tasks.Where(t => t.Resource == null).ToList();
                mainWindow.Tasks = new ObservableCollection<ProjectTask>(resultTaskList);

                var resultFulltaskList = context.Tasks.Where(t => t.Resource != null).ToList();
                mainWindow.FullTasks = new ObservableCollection<ProjectTask>(resultFulltaskList);
            }
        }
    }
}
