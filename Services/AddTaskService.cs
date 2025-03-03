using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;

namespace MSProj_Analog.Services
{
    public class AddTaskService : IAddTaskService
    {
        public void AddTask(AppDbContext context,ICollection<ProjectTask> tasks,  ProjectTask task)
        {
            tasks.Add(task);
            using (context)
            {
                context.Tasks.Add(task);
                context.SaveChanges();
            }
        }
    }
}