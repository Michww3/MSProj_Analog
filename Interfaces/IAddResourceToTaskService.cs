using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Interfaces
{
    public interface IAddResourceToTaskService
    {
        public void AddResourceToTask(AppDbContext context,ICollection<ProjectTask> projectTasks, ICollection<Resource> resources, int resId, int taskId);
    }
}
