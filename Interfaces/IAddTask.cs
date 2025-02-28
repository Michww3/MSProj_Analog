using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Interfaces
{
    public  interface IAddTask
    {
       public void AddTask(ICollection<ProjectTask> tasks,AppDbContext context, ProjectTask task);
    }
}
