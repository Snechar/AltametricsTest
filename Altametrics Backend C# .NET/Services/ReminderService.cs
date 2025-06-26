using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Altametrics_Backend_C__.NET.Data;
using System.Threading;
using System.Threading.Tasks;

public class ReminderService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ReminderService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public ReminderService(IServiceProvider services, ILogger<ReminderService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                var upcomingEvents = await db.Events
                    .Where(e => e.EventDate > DateTime.UtcNow &&
                                e.EventDate < DateTime.UtcNow.AddMinutes(30))
                    .ToListAsync(stoppingToken);

                foreach (var evt in upcomingEvents)
                {
                    var invitedEmails = db.RSVPs
                        .Where(r => r.EventCode == evt.EventCode)
                        .Select(r => r.Email)
                        .ToList();

                    // Assuming you'd log the non-responded emails somehow
                    _logger.LogInformation($"Upcoming event '{evt.Name}' has {invitedEmails.Count} RSVP(s).");

                    // Call email sending logic here - not implemented
                    // await SimulateReminderSend(invitedEmails);
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("ReminderService is stopping.");
    }
}