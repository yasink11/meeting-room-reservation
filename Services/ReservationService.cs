using Microsoft.EntityFrameworkCore;
using MeetingRoomReservation.API.Data;
using MeetingRoomReservation.API.DTOs;
using MeetingRoomReservation.API.Entities;
using MeetingRoomReservation.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRoomReservation.API.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReservationDto>> GetAllReservationsAsync()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomName = r.Room.Name,
                UserName = r.UserName,
                Title = r.Title,
                Description = r.Description,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ParticipantCount = r.ParticipantCount,
                RecurringGroupId = r.RecurringGroupId
            }).ToList();
        }

        public async Task<ReservationDto> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            return new ReservationDto
            {
                Id = reservation.Id,
                RoomId = reservation.RoomId,
                RoomName = reservation.Room.Name,
                UserName = reservation.UserName,
                Title = reservation.Title,
                Description = reservation.Description,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ParticipantCount = reservation.ParticipantCount,
                RecurringGroupId = reservation.RecurringGroupId
            };
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            // İş Kuralı 1: Oda var mı kontrol et
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new Exception("Oda bulunamadı");

            // İş Kuralı 2: Kapasite kontrolü
            if (dto.ParticipantCount > room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({room.Capacity}) aşıyor");

            // İş Kuralı 3: Çakışan rezervasyon kontrolü
            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.RoomId == dto.RoomId &&
                              r.StartTime < dto.EndTime &&
                              r.EndTime > dto.StartTime);

            if (hasConflict)
                throw new Exception("Bu saatte oda için başka bir rezervasyon var");

            // İş Kuralı 4: Kullanıcı aynı saatte başka rezervasyon yapamaz
            var userHasConflict = await _context.Reservations
                .AnyAsync(r => r.UserName == dto.UserName &&
                              r.StartTime < dto.EndTime &&
                              r.EndTime > dto.StartTime);

            if (userHasConflict)
                throw new Exception("Bu saatte zaten başka bir rezervasyonunuz var");

            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                UserName = dto.UserName,
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                ParticipantCount = dto.ParticipantCount,
                IsCancelled = false,
                CreatedDate = DateTime.Now
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new ReservationDto
            {
                Id = reservation.Id,
                RoomId = reservation.RoomId,
                RoomName = room.Name,
                UserName = reservation.UserName,
                Title = reservation.Title,
                Description = reservation.Description,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ParticipantCount = reservation.ParticipantCount
            };
        }

        public async Task<List<ReservationDto>> CreateRecurringReservationAsync(CreateRecurringReservationDto dto)
        {
            // Recurring Group oluştur
            var recurringGroup = new RecurringGroup
            {
                Pattern = dto.Pattern,
                Interval = dto.Interval,
                DayOfWeek = dto.DayOfWeek,
                StartDate = dto.StartTime.Date,
                EndDate = dto.StartTime.AddDays(dto.WeekCount * 7).Date,
                ExceptionDates = dto.ExceptionDates ?? "",
                CreatedDate = DateTime.Now
            };

            _context.RecurringGroups.Add(recurringGroup);
            await _context.SaveChangesAsync();

            // Exception tarihlerini parse et
            var exceptionDatesList = string.IsNullOrEmpty(dto.ExceptionDates)
                ? new List<DateTime>()
                : dto.ExceptionDates.Split(',')
                    .Select(d => DateTime.Parse(d.Trim()))
                    .ToList();

            // Her hafta için rezervasyon oluştur
            var reservations = new List<Reservation>();
            var currentDate = dto.StartTime;

            for (int i = 0; i < dto.WeekCount; i++)
            {
                // Exception tarihinde mi kontrol et
                if (!exceptionDatesList.Any(ed => ed.Date == currentDate.Date))
                {
                    var reservation = new Reservation
                    {
                        RoomId = dto.RoomId,
                        UserName = dto.UserName,
                        Title = dto.Title,
                        Description = dto.Description,
                        StartTime = currentDate,
                        EndTime = currentDate.AddHours((dto.EndTime - dto.StartTime).TotalHours),
                        ParticipantCount = dto.ParticipantCount,
                        RecurringGroupId = recurringGroup.Id,
                        IsCancelled = false,
                        CreatedDate = DateTime.Now
                    };

                    reservations.Add(reservation);
                }

                // Bir sonraki haftaya geç
                currentDate = currentDate.AddDays(7 * dto.Interval);
            }

            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

            // Room bilgisini çek
            var room = await _context.Rooms.FindAsync(dto.RoomId);

            return reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomName = room.Name,
                UserName = r.UserName,
                Title = r.Title,
                Description = r.Description,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ParticipantCount = r.ParticipantCount,
                RecurringGroupId = r.RecurringGroupId
            }).ToList();
        }

        public async Task<ReservationDto> UpdateReservationAsync(int id, UpdateReservationDto dto)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            // İş Kuralı: Toplantıdan 1 saat öncesine kadar güncellenebilir
            if (reservation.StartTime.AddHours(-1) < DateTime.Now)
                throw new Exception("Toplantıdan 1 saat öncesine kadar güncelleyebilirsiniz");

            // Kapasite kontrolü
            if (dto.ParticipantCount > reservation.Room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({reservation.Room.Capacity}) aşıyor");

            // Çakışma kontrolü (kendi rezervasyonu hariç)
            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.Id != id &&
                              r.RoomId == reservation.RoomId &&
                              r.StartTime < dto.EndTime &&
                              r.EndTime > dto.StartTime);

            if (hasConflict)
                throw new Exception("Bu saatte oda için başka bir rezervasyon var");

            reservation.Title = dto.Title;
            reservation.Description = dto.Description;
            reservation.StartTime = dto.StartTime;
            reservation.EndTime = dto.EndTime;
            reservation.ParticipantCount = dto.ParticipantCount;
            reservation.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ReservationDto
            {
                Id = reservation.Id,
                RoomId = reservation.RoomId,
                RoomName = reservation.Room.Name,
                UserName = reservation.UserName,
                Title = reservation.Title,
                Description = reservation.Description,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ParticipantCount = reservation.ParticipantCount,
                RecurringGroupId = reservation.RecurringGroupId
            };
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return false;

            // İş Kuralı: Toplantıdan 1 saat öncesine kadar iptal edilebilir
            if (reservation.StartTime.AddHours(-1) < DateTime.Now)
                throw new Exception("Toplantıdan 1 saat öncesine kadar iptal edebilirsiniz");

            reservation.IsCancelled = true;
            reservation.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReservationDto>> GetConflictingReservationsAsync(int roomId, DateTime start, DateTime end)
        {
            var conflicts = await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.RoomId == roomId &&
                           r.StartTime < end &&
                           r.EndTime > start)
                .ToListAsync();

            return conflicts.Select(r => new ReservationDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomName = r.Room.Name,
                UserName = r.UserName,
                Title = r.Title,
                Description = r.Description,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                ParticipantCount = r.ParticipantCount,
                RecurringGroupId = r.RecurringGroupId
            }).ToList();
        }
    }
}