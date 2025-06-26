using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Altametrics_Backend_C__.NET.Models.Entities
{
    public class RSVP
    {
        [Key]
        public int RsvpId { get; set; }
            public int EventId { get; set; }
        public Guid EventCode { get; set; }
        public string GuestName { get; set; } = default!;
        public string Email { get; set; } = default!; 
        public int GuestCount { get; set; } = 1;
        public string ResponseStatus { get; set; } = default!;
        public bool ReminderRequested { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
