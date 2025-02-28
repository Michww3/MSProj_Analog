using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Interfaces
{
    public  interface IAddTaskService
    {
       public void AddTask(ICollection<ProjectTask> tasks,AppDbContext context, ProjectTask task);
    }
}
