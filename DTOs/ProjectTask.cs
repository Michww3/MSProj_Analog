using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using System.Xml.Serialization;
namespace MSProj_Analog.DTOs
{
    [Serializable]
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
        [Key,XmlAttribute]
        public int Id { get; set; }

        [Required,MaxLength(30), XmlAttribute]
        public string Name { get; set; }

        [Required, XmlAttribute]
        public DateTime StartDate { get; set; }

        [Required, XmlAttribute]
        public DateTime EndDate { get; set; }

        [ForeignKey("ResourceId"),XmlIgnore]
        public Resource? Resource { get; set; }
        [XmlIgnore]
        public int? ResourceId { get; set; }

    }
}