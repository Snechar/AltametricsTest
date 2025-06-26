namespace Altametrics_Backend_C__.NET.Models.DTOs.RSVP
{
    public class RSVPRespModel
    {
        public int RsvpId { get; set; }
        public Guid EventCode { get; set; }
        public string Email { get; set; } = default!; 
        public string GuestName { get; set; }
        public int GuestCount { get; set; }
        public string ResponseStatus { get; set; }
        public bool ReminderRequested { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
