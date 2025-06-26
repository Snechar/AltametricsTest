using System.ComponentModel.DataAnnotations;

namespace Altametrics_Backend_C__.NET.Models.DTOs.Event
{
    public class CreateEventModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
    }
}
