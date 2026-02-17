using Microsoft.EntityFrameworkCore;
using MeetingRoomReservation.API.Entities;
using System;

namespace MeetingRoomReservation.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RecurringGroup> RecurringGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Room Configuration
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Equipment).HasMaxLength(500);
                entity.HasIndex(e => e.Name);
                entity.HasQueryFilter(e => e.IsActive); // Soft delete filter
            });

            // Reservation Configuration
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                
                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Reservations)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RecurringGroup)
            .WithMany(rg => rg.Reservations)
            .HasForeignKey(e => e.RecurringGroupId)
            .OnDelete(DeleteBehavior.Cascade)  
            .IsRequired(false);  

                entity.HasIndex(e => new { e.RoomId, e.StartTime, e.EndTime });
                entity.HasQueryFilter(e => !e.IsCancelled); // Cancelled olanları otomatik filtrele
            });

            // RecurringGroup Configuration
            modelBuilder.Entity<RecurringGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Pattern).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ExceptionDates).HasMaxLength(2000);
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Toplantı Odası A",
                    Capacity = 10,
                    Floor = 1,
                    Equipment = "Projeksiyon,Beyaz Tahta,Telefon",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Room
                {
                    Id = 2,
                    Name = "Toplantı Odası B",
                    Capacity = 6,
                    Floor = 2,
                    Equipment = "Projeksiyon,Telefon",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new Room
                {
                    Id = 3,
                    Name = "Konferans Salonu",
                    Capacity = 50,
                    Floor = 3,
                    Equipment = "Projeksiyon,Ses Sistemi,Mikrofon,Beyaz Tahta",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}