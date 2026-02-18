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

        // DB'den gelen DateTime'ı UTC olarak işaretle
        private static DateTime AsUtc(DateTime dt) =>
            DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        // Client'tan gelen DateTime'ı UTC'ye normalize et
        // Client "Z" gönderdiyse zaten UTC'dir, local gönderdiyse convert edilir
        private static DateTime ToUtc(DateTime dt) =>
            dt.Kind == DateTimeKind.Utc ? dt : dt.ToUniversalTime();

        private static ReservationDto MapToDto(Reservation r) => new ReservationDto
        {
            Id = r.Id,
            RoomId = r.RoomId,
            RoomName = r.Room?.Name,
            UserName = r.UserName,
            Title = r.Title,
            Description = r.Description,
            StartTime = AsUtc(r.StartTime),
            EndTime = AsUtc(r.EndTime),
            ParticipantCount = r.ParticipantCount,
            RecurringGroupId = r.RecurringGroupId
        };

        public async Task<List<ReservationDto>> GetAllReservationsAsync(
            int? roomId = null,
            string userName = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Reservations
                .Include(r => r.Room)
                .AsQueryable();

            if (roomId.HasValue)
                query = query.Where(r => r.RoomId == roomId.Value);

            if (!string.IsNullOrEmpty(userName))
                query = query.Where(r => r.UserName.Contains(userName));

            // Filtre tarihlerini de UTC'ye normalize et
            if (startDate.HasValue)
                query = query.Where(r => r.StartTime >= ToUtc(startDate.Value));

            if (endDate.HasValue)
                query = query.Where(r => r.StartTime <= ToUtc(endDate.Value));

            var reservations = await query
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return reservations.Select(MapToDto).ToList();
        }

        public async Task<ReservationDto> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            return MapToDto(reservation);
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new Exception("Oda bulunamadı");

            if (dto.ParticipantCount > room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({room.Capacity}) aşıyor");

            // Client'tan gelen tarihleri UTC'ye normalize et
            var startUtc = ToUtc(dto.StartTime);
            var endUtc = ToUtc(dto.EndTime);

            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.RoomId == dto.RoomId &&
                               !r.IsCancelled &&
                               r.StartTime < endUtc &&
                               r.EndTime > startUtc);

            if (hasConflict)
                throw new Exception("Bu saatte oda için başka bir rezervasyon var");

            var userHasConflict = await _context.Reservations
                .AnyAsync(r => r.UserName == dto.UserName &&
                               !r.IsCancelled &&
                               r.StartTime < endUtc &&
                               r.EndTime > startUtc);

            if (userHasConflict)
                throw new Exception("Bu saatte zaten başka bir rezervasyonunuz var");

            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                UserName = dto.UserName,
                Title = dto.Title,
                Description = dto.Description,
                StartTime = startUtc,
                EndTime = endUtc,
                ParticipantCount = dto.ParticipantCount,
                IsCancelled = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            reservation.Room = room;
            return MapToDto(reservation);
        }

        public async Task<List<ReservationDto>> CreateRecurringReservationAsync(CreateRecurringReservationDto dto)
        {
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new Exception("Oda bulunamadı");

            if (dto.ParticipantCount > room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({room.Capacity}) aşıyor");

            var startUtc = ToUtc(dto.StartTime);
            var endUtc = ToUtc(dto.EndTime);
            var duration = endUtc - startUtc;

            var exceptionDatesList = string.IsNullOrEmpty(dto.ExceptionDates)
                ? new List<DateTime>()
                : dto.ExceptionDates.Split(',')
                    .Select(d => ToUtc(DateTime.Parse(d.Trim())).Date)
                    .ToList();

            var recurringGroup = new RecurringGroup
            {
                Pattern = dto.Pattern,
                Interval = dto.Interval,
                DayOfWeek = dto.DayOfWeek,
                StartDate = startUtc.Date,
                EndDate = startUtc.AddDays(dto.WeekCount * 7).Date,
                ExceptionDates = dto.ExceptionDates ?? "",
                CreatedDate = DateTime.UtcNow
            };

            _context.RecurringGroups.Add(recurringGroup);
            await _context.SaveChangesAsync();

            var reservations = new List<Reservation>();
            var currentStart = startUtc;

            for (int i = 0; i < dto.WeekCount; i++)
            {
                // Exception tarihlerini UTC date olarak karşılaştır
                if (!exceptionDatesList.Any(ed => ed == currentStart.Date))
                {
                    var currentEnd = currentStart + duration;

                    var hasRoomConflict = await _context.Reservations
                        .AnyAsync(r => r.RoomId == dto.RoomId &&
                                       !r.IsCancelled &&
                                       r.StartTime < currentEnd &&
                                       r.EndTime > currentStart);

                    if (hasRoomConflict)
                        throw new Exception($"{currentStart:dd.MM.yyyy} tarihinde bu odada çakışan rezervasyon var");

                    var hasUserConflict = await _context.Reservations
                        .AnyAsync(r => r.UserName == dto.UserName &&
                                       !r.IsCancelled &&
                                       r.StartTime < currentEnd &&
                                       r.EndTime > currentStart);

                    if (hasUserConflict)
                        throw new Exception($"{currentStart:dd.MM.yyyy} tarihinde kullanıcının başka rezervasyonu var");

                    reservations.Add(new Reservation
                    {
                        RoomId = dto.RoomId,
                        UserName = dto.UserName,
                        Title = dto.Title,
                        Description = dto.Description,
                        StartTime = currentStart,
                        EndTime = currentEnd,
                        ParticipantCount = dto.ParticipantCount,
                        RecurringGroupId = recurringGroup.Id,
                        IsCancelled = false,
                        CreatedDate = DateTime.UtcNow
                    });
                }

                currentStart = currentStart.AddDays(7 * dto.Interval);
            }

            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

            return reservations.Select(r => { r.Room = room; return MapToDto(r); }).ToList();
        }

        public async Task<ReservationDto> UpdateReservationAsync(int id, UpdateReservationDto dto)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return null;

            // Zaman karşılaştırmasında UTC kullan
            if (AsUtc(reservation.StartTime).AddHours(-1) < DateTime.UtcNow)
                throw new Exception("Toplantıdan 1 saat öncesine kadar güncelleyebilirsiniz");

            if (dto.ParticipantCount > reservation.Room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({reservation.Room.Capacity}) aşıyor");

            var startUtc = ToUtc(dto.StartTime);
            var endUtc = ToUtc(dto.EndTime);

            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.Id != id &&
                               r.RoomId == reservation.RoomId &&
                               !r.IsCancelled &&
                               r.StartTime < endUtc &&
                               r.EndTime > startUtc);

            if (hasConflict)
                throw new Exception("Bu saatte oda için başka bir rezervasyon var");

            reservation.Title = dto.Title;
            reservation.Description = dto.Description;
            reservation.StartTime = startUtc;
            reservation.EndTime = endUtc;
            reservation.ParticipantCount = dto.ParticipantCount;
            reservation.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(reservation);
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return false;

            if (AsUtc(reservation.StartTime).AddHours(-1) < DateTime.UtcNow)
                throw new Exception("Toplantıdan 1 saat öncesine kadar iptal edebilirsiniz");

            reservation.IsCancelled = true;
            reservation.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReservationDto>> GetConflictingReservationsAsync(int roomId, DateTime start, DateTime end)
        {
            var startUtc = ToUtc(start);
            var endUtc = ToUtc(end);

            var conflicts = await _context.Reservations
                .Include(r => r.Room)
                .Where(r => r.RoomId == roomId &&
                            !r.IsCancelled &&
                            r.StartTime < endUtc &&
                            r.EndTime > startUtc)
                .ToListAsync();

            return conflicts.Select(MapToDto).ToList();
        }
    }
}