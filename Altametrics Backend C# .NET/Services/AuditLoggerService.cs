using Altametrics_Backend_C__.NET.Data;
using Altametrics_Backend_C__.NET.Models.Entities;

namespace Altametrics_Backend_C__.NET.Services
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, int? eventId = null, int? userId = null, string? email = null);
    }

    public class AuditLogger : IAuditLogger
    {
        private readonly AppDBContext _context;

        public AuditLogger(AppDBContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string action, int? eventId = null, int? userId = null, string? email = null)
        {
            var log = new AuditLog
            {
                EventId = eventId,
                UserId = userId,
                Email = email,
                Action = action,
                LogTime = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
