using System.ComponentModel.DataAnnotations;

namespace Altametrics_Backend_C__.NET.Models.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }
        public int? EventId { get; set; }
        public int? UserId { get; set; }
        public string? Email { get; set; } 
        public string Action { get; set; } = default!;
        public DateTime LogTime { get; set; }
    }
}
