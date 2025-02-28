using MSProj_Analog.DTOs;

namespace MSProj_Analog.Interfaces
{
    public interface IAddResourceToTask
    {
        public void AddResourceToTask(ICollection<ProjectTask> projectTasks, ICollection<Resource> resources, int resId, int taskId);
    }
}
