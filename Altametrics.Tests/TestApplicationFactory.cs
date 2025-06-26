using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Altametrics_Backend_C__.NET.Data;
using Microsoft.AspNetCore.Hosting;
using Altametrics_Backend_C__.NET.Models.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class TestApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); // Important!
        builder.ConfigureServices(services =>
        {
            // Remove existing DB context registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDBContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Register in-memory DB context
            services.AddDbContext<AppDBContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Build service provider and seed data
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            db.Database.EnsureCreated();

            // Seed user 
            var user = new User
            {

                Email = "owner@example.com",
                PasswordHash = "f11626b5b3fa789aacd5548267668f50c3283b9665b778226b4951a52a6c8ece"
            };
            db.Users.Add(user);
            db.SaveChanges();

            // Seed Event
            var testEvent = new Event
            {
                EventId = 0,
                UserId = user.UserId,
                Name = "Seeded Test Event",
                Description = "For testing",
                EventDate = DateTime.UtcNow.AddDays(1),
                Location = "Test Location",
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000000")
            };
            db.Events.Add(testEvent);
            db.SaveChanges();
            testEvent = new Event
            {
                EventId = 0,
                UserId = user.UserId,
                Name = "Seeded Test Event",
                Description = "For testing",
                EventDate = DateTime.UtcNow.AddDays(1),
                Location = "Test Location",
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };
            db.Events.Add(testEvent);
            db.SaveChanges();
            testEvent = new Event
            {
                EventId = 0,
                UserId = user.UserId,
                Name = "Seeded Test Event",
                Description = "For testing",
                EventDate = DateTime.UtcNow.AddDays(1),
                Location = "Test Location",
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000002")
            };
            db.Events.Add(testEvent);
            db.SaveChanges();
            testEvent = new Event
            {
                EventId = 0,
                UserId = user.UserId,
                Name = "Seeded Test Event",
                Description = "For testing",
                EventDate = DateTime.UtcNow.AddDays(1),
                Location = "Test Location",
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000003")
            };
            db.Events.Add(testEvent);
            db.SaveChanges();
            var rsvp = new RSVP
            {
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                GuestName = "Diana",
                Email = "diana@example.com",
                GuestCount = 1,
                ResponseStatus = "Going",
                ReminderRequested = true
            };
            db.RSVPs.Add(rsvp);
            db.SaveChanges();
            rsvp = new RSVP
            {
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                GuestName = "Diana",
                Email = "diana@example.com",
                GuestCount = 1,
                ResponseStatus = "Going",
                ReminderRequested = true
            };
            db.RSVPs.Add(rsvp);
            db.SaveChanges();
            rsvp = new RSVP
            {
                EventCode = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                GuestName = "Diana",
                Email = "diana@example.com",
                GuestCount = 1,
                ResponseStatus = "Going",
                ReminderRequested = true
            };
            db.RSVPs.Add(rsvp);


            db.SaveChanges();
            Console.WriteLine("Seeded Events: " + db.Events.Count());
            Console.WriteLine("Seeded RSVPs: " + db.RSVPs.Count());
        });
    }
}