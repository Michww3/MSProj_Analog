﻿using MSProj_Analog.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSProj_Analog.DTOs
{
    public class Resource
    {
        public Resource(ResourceType resourceType, string name, decimal standardRate, decimal? overtimeRate = null)
        {
            Name = name;
            StandardRate = standardRate;
            OvertimeRate = resourceType == ResourceType.Labor ? overtimeRate : null;
            Type = resourceType;
        }

        private Resource() { }

        [Key]
        public int Id { get; set; }
        [Required,MaxLength(30)]
        public string Name { get; private set; }
        [Required]
        public decimal StandardRate { get; private set; }

        public decimal? OvertimeRate { get; private set; }

        [Required]
        public ResourceType Type { get; private set; }

        [ForeignKey("ProjectTaskId")]
        public ProjectTask? ProjectTask { get; set; }
        public int? ProjectTaskId { get; set; }
        //public List<ProjectTask>? AppointedTasks { get; set; }

        public override string ToString()
        {
            return $"{Id}. Name: {Name} , Type: {Type}";
        }
    }
}