using MSProj_Analog.Enums;
using System.ComponentModel.DataAnnotations;

namespace MSProj_Analog.Helpers
{
    internal class AddResourceValidateModel
    {
        [Required(ErrorMessage = "Resource name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Resource name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Standard rate must be a positive number.")]
        public decimal StandardRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Overtime rate must be a positive number.")]
        public decimal? OvertimeRate { get; set; }

        [EnumDataType(typeof(ResourceType), ErrorMessage = "Invalid resource type.")]
        public ResourceType ResourceType { get; set; }
    }
}
