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
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return Ok(ApiResponse<object>.SuccessResult(rooms, "Odalar başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }

        // GET: api/rooms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(id);

                if (room == null)
                    return NotFound(ApiResponse<object>.FailResult("Oda bulunamadı"));

                return Ok(ApiResponse<object>.SuccessResult(room, "Oda başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

                var room = await _roomService.CreateRoomAsync(dto);
                return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, ApiResponse<object>.SuccessResult(room, "Oda başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }

        // PUT: api/rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

                var room = await _roomService.UpdateRoomAsync(id, dto);

                if (room == null)
                    return NotFound(ApiResponse<object>.FailResult("Oda bulunamadı"));

                return Ok(ApiResponse<object>.SuccessResult(room, "Oda başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }

        // DELETE: api/rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var result = await _roomService.DeleteRoomAsync(id);

                if (!result)
                    return NotFound(ApiResponse<object>.FailResult("Oda bulunamadı"));

                return Ok(ApiResponse<object>.SuccessResult(null, "Oda başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }
    }
}