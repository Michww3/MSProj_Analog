using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MSProj_Analog.DTOs
{
    public class ProjectTask
    {
        public ProjectTask(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            //AssignedResource = new List<Resource>();
        }

        private ProjectTask() 
        {
            //AssignedResource = new List<Resource>();
        }
        [Key]
        public int Id { get; set; }

        [Required,MaxLength(30)]
        public string Name { get; private set; }

        [Required]
        public DateTime StartDate { get; private set; }

        [Required]
        public DateTime EndDate { get; private set; }

        [ForeignKey("ResourceId")]
        public Resource? Resource { get; set; }
        public int? ResourceId { get; set; }

        //[Required]
        //public List<Resource> AssignedResource { get; set; }

        //public ProjectTask? PreviousTask { get; private set; }
        //public ProjectTask? NextTask { get; private set; }
    }
}