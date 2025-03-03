using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Interfaces
{
    public  interface IAddTaskService
    {
       public void AddTask(AppDbContext context, ICollection<ProjectTask> tasks, ProjectTask task);
    }
}
