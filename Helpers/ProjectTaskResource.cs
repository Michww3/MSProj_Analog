using MSProj_Analog.DTOs;

namespace MSProj_Analog.Helpers
{
    public class ProjectTaskResource
    {
        public int ProjectTaskId { get; set; }
        public ProjectTask ProjectTask { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}
