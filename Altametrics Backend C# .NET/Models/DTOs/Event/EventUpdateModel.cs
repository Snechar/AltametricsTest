namespace Altametrics_Backend_C__.NET.Models.DTOs.Event
{
    public class EventUpdateModel
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
    }
}
