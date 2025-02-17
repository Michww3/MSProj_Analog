namespace MSProj_Analog.DTOs
{
    public class ProjectTask
    {
        public ProjectTask(Resource resource, string name, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid();
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            AssignedResource.Add(resource);
        }

        private ProjectTask() { }

        public Guid Id { get; set; }
        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<Resource> AssignedResource { get; set; }
        public ProjectTask? PreviousTask { get; private set; }
        public ProjectTask? NextTask { get; private set; }
    }
}