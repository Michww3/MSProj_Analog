using MSProj_Analog.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace MSProj_Analog.DTOs
{
    [Serializable]
    public class Resource
    {
        public Resource(ResourceType resourceType, string name, decimal standardRate, decimal? overtimeRate = null)
        {
            Name = name;
            StandardRate = standardRate;
            OvertimeRate = resourceType == ResourceType.Labor ? overtimeRate : null;
            Type = resourceType;
        }

        public Resource() { }

        [Key, XmlAttribute]
        public int Id { get; set; }

        [Required,MaxLength(30), XmlAttribute]
        public string Name { get; set; }

        [Required, XmlAttribute]
        public decimal StandardRate { get; set; }

        [XmlElement]
        public decimal? OvertimeRate { get; set; }

        [Required, XmlAttribute]
        public ResourceType Type { get; set; }

        [ForeignKey("ProjectTaskId"), XmlIgnore]
        public ProjectTask? ProjectTask { get; set; }

        [XmlIgnore]
        public int? ProjectTaskId { get; set; }

        public override string ToString()
        {
            return $"{Id}. Name: {Name} , Type: {Type}";
        }
    }
}