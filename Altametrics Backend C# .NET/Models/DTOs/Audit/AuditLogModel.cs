namespace Altametrics_Backend_C__.NET.Models.DTOs.Audit
{
    public class AuditLogModel
    {
        public int AuditId { get; set; }
        public int? EventId { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; }
        public DateTime LogTime { get; set; }
    }
}
