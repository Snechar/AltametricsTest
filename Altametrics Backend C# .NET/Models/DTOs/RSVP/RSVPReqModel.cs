using System.ComponentModel.DataAnnotations;

namespace Altametrics_Backend_C__.NET.Models.DTOs.RSVP
{
    public class RSVPReqModel
    {
        [Required]
        public Guid EventCode { get; set; }

        [Required]
        public string GuestName { get; set; } = default!;

        [Range(1, 10)]
        public int GuestCount { get; set; } = 1;

        [Required]
        [RegularExpression("Going|Not Going|Maybe", ErrorMessage = "Invalid response status.")]
        public string ResponseStatus { get; set; } = default!;

        public bool ReminderRequested { get; set; } = false;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
    }
}
