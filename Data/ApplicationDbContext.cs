using Microsoft.EntityFrameworkCore;
using MeetingRoomReservation.API.Entities;
using System;
using System.Collections.Generic;

namespace MeetingRoomReservation.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Equipment> Equipments { get; set; } 
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RecurringGroup> RecurringGroups { get; set; }
        public DbSet<RecurringGroupExceptionDate> RecurringGroupExceptionDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Equipment Konfigürasyonu
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique(); 
            });

            // 2. Room Konfigürasyonu (Normalize Edildi)
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name);
                entity.HasQueryFilter(e => e.IsActive);

                // Many-to-Many İlişki Tanımı
                entity.HasMany(r => r.Equipments)
                      .WithMany(e => e.Rooms)
                      .UsingEntity<Dictionary<string, object>>(
                          "RoomEquipments", // Ara tablo adı
                          j => j.HasOne<Equipment>().WithMany().HasForeignKey("EquipmentId"),
                          j => j.HasOne<Room>().WithMany().HasForeignKey("RoomId")
                      );
            });

            // 3. Reservation Konfigürasyonu
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
                entity.HasQueryFilter(e => !e.IsCancelled);
            });

            // 4. Recurring Konfigürasyonları
            modelBuilder.Entity<RecurringGroup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Pattern).IsRequired().HasMaxLength(50);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
            });

            modelBuilder.Entity<RecurringGroupExceptionDate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.RecurringGroup)
                      .WithMany(r => r.ExceptionDates)
                      .HasForeignKey(e => e.RecurringGroupId);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var projeksiyon = new { Id = 1, Name = "Projeksiyon" };
            var beyazTahta = new { Id = 2, Name = "Beyaz Tahta" };
            var telefon = new { Id = 3, Name = "Telefon" };
            var sesSistemi = new { Id = 4, Name = "Ses Sistemi" };
            var mikrofon = new { Id = 5, Name = "Mikrofon" };

            modelBuilder.Entity<Equipment>().HasData(projeksiyon, beyazTahta, telefon, sesSistemi, mikrofon);

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Toplantı Odası A", Capacity = 10, Floor = 1, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Room { Id = 2, Name = "Toplantı Odası B", Capacity = 6, Floor = 2, IsActive = true, CreatedDate = DateTime.UtcNow },
                new Room { Id = 3, Name = "Konferans Salonu", Capacity = 50, Floor = 3, IsActive = true, CreatedDate = DateTime.UtcNow }
            );

            modelBuilder.Entity("RoomEquipments").HasData(
                new { RoomId = 1, EquipmentId = 1 },
                new { RoomId = 1, EquipmentId = 2 },
                new { RoomId = 1, EquipmentId = 3 },
                new { RoomId = 2, EquipmentId = 1 },
                new { RoomId = 2, EquipmentId = 3 },
                new { RoomId = 3, EquipmentId = 1 },
                new { RoomId = 3, EquipmentId = 2 },
                new { RoomId = 3, EquipmentId = 4 },
                new { RoomId = 3, EquipmentId = 5 }
            );
        }
    }
}