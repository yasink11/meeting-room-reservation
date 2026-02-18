using MeetingRoomReservation.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomReservation.API.Services.Interfaces
{
    public interface IRoomService
    {
        Task<List<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto> GetRoomByIdAsync(int id);
        Task<RoomDto> CreateRoomAsync(CreateRoomDto dto);
        Task<RoomDto> UpdateRoomAsync(int id, UpdateRoomDto dto);
        Task<bool> DeleteRoomAsync(int id);

        Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime start, DateTime end);
    }
}