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

            if (startDate.HasValue)
                query = query.Where(r => r.StartTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.StartTime <= endDate.Value);

            var reservations = await query
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

            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new Exception("Oda bulunamadı");

            if (dto.ParticipantCount > room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({room.Capacity}) aşıyor");


            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.RoomId == dto.RoomId &&
                              r.StartTime < dto.EndTime &&
                              r.EndTime > dto.StartTime);

            if (hasConflict)
                throw new Exception("Bu saatte oda için başka bir rezervasyon var");

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

            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                throw new Exception("Oda bulunamadı");

            if (dto.ParticipantCount > room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({room.Capacity}) aşıyor");


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


            var exceptionDatesList = string.IsNullOrEmpty(dto.ExceptionDates)
                ? new List<DateTime>()
                : dto.ExceptionDates.Split(',')
                    .Select(d => DateTime.Parse(d.Trim()))
                    .ToList();


            var reservations = new List<Reservation>();
            var currentDate = dto.StartTime;

            for (int i = 0; i < dto.WeekCount; i++)
            {

                if (!exceptionDatesList.Any(ed => ed.Date == currentDate.Date))
                {
                    var endTime = currentDate.AddHours((dto.EndTime - dto.StartTime).TotalHours);


                    var hasRoomConflict = await _context.Reservations
                        .AnyAsync(r => r.RoomId == dto.RoomId &&
                                      r.StartTime < endTime &&
                                      r.EndTime > currentDate);

                    if (hasRoomConflict)
                        throw new Exception($"{currentDate:dd.MM.yyyy} tarihinde bu odada çakışan rezervasyon var");


                    var hasUserConflict = await _context.Reservations
                        .AnyAsync(r => r.UserName == dto.UserName &&
                                      r.StartTime < endTime &&
                                      r.EndTime > currentDate);

                    if (hasUserConflict)
                        throw new Exception($"{currentDate:dd.MM.yyyy} tarihinde kullanıcının başka rezervasyonu var");

                    var reservation = new Reservation
                    {
                        RoomId = dto.RoomId,
                        UserName = dto.UserName,
                        Title = dto.Title,
                        Description = dto.Description,
                        StartTime = currentDate,
                        EndTime = endTime,
                        ParticipantCount = dto.ParticipantCount,
                        RecurringGroupId = recurringGroup.Id,
                        IsCancelled = false,
                        CreatedDate = DateTime.Now
                    };

                    reservations.Add(reservation);
                }

      
                currentDate = currentDate.AddDays(7 * dto.Interval);
            }

            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

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

            if (reservation.StartTime.AddHours(-1) < DateTime.Now)
                throw new Exception("Toplantıdan 1 saat öncesine kadar güncelleyebilirsiniz");

            if (dto.ParticipantCount > reservation.Room.Capacity)
                throw new Exception($"Katılımcı sayısı ({dto.ParticipantCount}) oda kapasitesini ({reservation.Room.Capacity}) aşıyor");

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