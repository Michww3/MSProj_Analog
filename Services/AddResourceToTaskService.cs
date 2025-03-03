using MSProj_Analog.Config;
using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using Xceed.Wpf.Toolkit;

namespace MSProj_Analog.Services
{
    public class AddResourceToTaskService : IAddResourceToTaskService
    {
        public void AddResourceToTask(AppDbContext context,ICollection<ProjectTask> tasks, ICollection<Resource> resources, int resourceId, int taskId)
        {
            var res = resources.FirstOrDefault(r => r.Id == resourceId);
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            using(context)
            {
                var dbRes = context.Resources.SingleOrDefault(r => r.Id == resourceId);
                var dbTask = context.Tasks.SingleOrDefault(t => t.Id == taskId);

                if (dbRes != null && dbTask != null)
                {
                    dbRes.ProjectTask = dbTask;
                    dbTask.Resource = dbRes;
                    context.SaveChanges();
                    tasks.Remove(task);
                    resources.Remove(res);
                }
                else
                {
                    MessageBox.Show(ConfigOptions.Messages.InvalidData);
                }
            }
        }
    }
}