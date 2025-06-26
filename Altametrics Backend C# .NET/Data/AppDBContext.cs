using Altametrics_Backend_C__.NET.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using Microsoft.EntityFrameworkCore;

namespace Altametrics_Backend_C__.NET.Data
{
    public class AppDBContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("users")
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Event>()
                .ToTable("events")
                .HasIndex(e => e.EventCode)
                .IsUnique();

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.EventDate);

            modelBuilder.Entity<RSVP>()
                 .ToTable("rsvp")
                 .HasIndex(r => new { r.EventCode, r.GuestName }).IsUnique(); ; 

            modelBuilder.Entity<AuditLog>()
                .ToTable("auditlog")
                .HasIndex(a => a.EventId);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => a.UserId);


            //Mapping proprieties to columns, sice C# assumes that field names are the same as table column names, and C# naming conventions do not match PostgreSQL naming conventions

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");

                entity.HasKey(e => e.EventId);
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.EventDate).HasColumnName("event_date");
                entity.Property(e => e.Location).HasColumnName("location");
                entity.Property(e => e.EventCode).HasColumnName("event_code");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                modelBuilder.Entity<Event>()
                    .HasIndex(e => e.EventCode)
                    .IsUnique();
                entity.HasIndex(e => e.EventDate);
            });


            modelBuilder.Entity<RSVP>(entity =>
            {
                entity.ToTable("rsvp");

                entity.HasKey(r => r.RsvpId);
                entity.Property(r => r.RsvpId).HasColumnName("rsvp_id");
                entity.Property(r => r.EventId).HasColumnName("event_id");
                entity.Property(r => r.EventCode).HasColumnName("event_code");
                entity.Property(r => r.GuestName).HasColumnName("guest_name");
                entity.Property(r => r.Email).HasColumnName("email");
                entity.Property(r => r.GuestCount).HasColumnName("guest_count");
                entity.Property(r => r.ResponseStatus).HasColumnName("response_status");
                entity.Property(r => r.ReminderRequested).HasColumnName("reminder_requested");
                entity.Property(r => r.CreatedAt).HasColumnName("created_at");

                entity.HasIndex(r => new { r.EventCode, r.GuestName }).IsUnique();
            });
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("auditlog");
                entity.HasKey(a => a.AuditId);
                entity.Property(a => a.AuditId).HasColumnName("audit_id");
                entity.Property(a => a.EventId).HasColumnName("event_id");
                entity.Property(a => a.UserId).HasColumnName("user_id");
                entity.Property(a => a.Email).HasColumnName("email"); 
                entity.Property(a => a.Action).HasColumnName("action");
                entity.Property(a => a.LogTime).HasColumnName("log_time");

                entity.HasIndex(a => a.EventId);
                entity.HasIndex(a => a.UserId);
            });


        }
    }
}
