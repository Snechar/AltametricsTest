namespace Altametrics_Backend_C__.NET.Models.DTOs.Event
{
    public class EventRespModel
    {
        public int EventId { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public Guid EventCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
    }
}
