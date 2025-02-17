using MSProj_Analog.Enums;

namespace MSProj_Analog.DTOs
{
    public class Resource
    {
        public Resource(ResourceType resourceType, string name, decimal standardRate, decimal? overtimeRate = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            StandardRate = standardRate;
            OvertimeRate = resourceType == ResourceType.Labor ? overtimeRate : null;
            Type = resourceType;
        }

        private Resource() { }

        public Guid Id { get; set; }
        public string Name { get; private set; }
        public decimal StandardRate { get; private set; }
        public decimal? OvertimeRate { get; private set; }
        public ResourceType Type { get; private set; }
        public ProjectTask? AppointedTask { get; set; }
        //public List<ProjectTask>? AppointedTasks { get; set; }
    }
}