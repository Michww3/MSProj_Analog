using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using System.Xml.Serialization;
namespace MSProj_Analog.DTOs
{
    public class ProjectTask
    {
        public ProjectTask(string name, DateTime startDate, DateTime endDate)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
        }

        public ProjectTask()
        { }
        [Key, XmlAttribute]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("ResourceId")]
        public Resource? Resource { get; set; }
        public int? ResourceId { get; set; }

    }
}