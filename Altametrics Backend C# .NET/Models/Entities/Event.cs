using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Altametrics_Backend_C__.NET.Models.Entities
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string Location { get; set; }

        [Required]
        public Guid EventCode { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RSVP> RSVPs { get; set; }
    }
}
