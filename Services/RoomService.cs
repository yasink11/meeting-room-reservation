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
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _context.Rooms.ToListAsync();

            return rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Floor = r.Floor,
                Equipment = string.IsNullOrEmpty(r.Equipment)
                    ? new List<string>()
                    : r.Equipment.Split(',').ToList()
            }).ToList();
        }

        public async Task<RoomDto> GetRoomByIdAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return null;

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Equipment = string.IsNullOrEmpty(room.Equipment)
                    ? new List<string>()
                    : room.Equipment.Split(',').ToList()
            };
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto dto)
        {
            var room = new Room
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                Floor = dto.Floor,
                Equipment = string.Join(",", dto.Equipment),
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Equipment = dto.Equipment
            };
        }

        public async Task<RoomDto> UpdateRoomAsync(int id, UpdateRoomDto dto)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return null;

            room.Name = dto.Name;
            room.Capacity = dto.Capacity;
            room.Floor = dto.Floor;
            room.Equipment = string.Join(",", dto.Equipment);
            room.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Equipment = dto.Equipment
            };
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return false;

            // Soft delete
            room.IsActive = false;
            room.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}