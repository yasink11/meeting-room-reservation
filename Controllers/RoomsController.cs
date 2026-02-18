using Microsoft.AspNetCore.Mvc;
using MeetingRoomReservation.API.DTOs;
using MeetingRoomReservation.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MeetingRoomReservation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // GET: api/rooms
        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            return Ok(ApiResponse<object>.SuccessResult(rooms, "Odalar başarıyla getirildi"));
        }

        // GET: api/rooms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);

            if (room == null)
                return NotFound(ApiResponse<object>.FailResult("Oda bulunamadı"));

            return Ok(ApiResponse<object>.SuccessResult(room, "Oda başarıyla getirildi"));
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

            var room = await _roomService.CreateRoomAsync(dto);
            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id },
                ApiResponse<object>.SuccessResult(room, "Oda başarıyla oluşturuldu"));
        }

        // PUT: api/rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

            var room = await _roomService.UpdateRoomAsync(id, dto);

            if (room == null)
                return NotFound(ApiResponse<object>.FailResult("Oda bulunamadı"));

            return Ok(ApiResponse<object>.SuccessResult(room, "Oda başarıyla güncellendi"));
        }

        // DELETE: api/rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.FailResult("Oda bulunamadı"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Oda başarıyla silindi"));
        }

        // GET: api/rooms/available?start=2025-03-03T10:00:00&end=2025-03-03T11:00:00
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            if (start >= end)
                return BadRequest(ApiResponse<object>.FailResult("Başlangıç tarihi bitiş tarihinden önce olmalıdır"));

            var rooms = await _roomService.GetAvailableRoomsAsync(start, end);
            return Ok(ApiResponse<object>.SuccessResult(rooms, "Müsait odalar getirildi"));
        }
    }
}