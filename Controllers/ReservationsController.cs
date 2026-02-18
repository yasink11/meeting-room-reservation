using Microsoft.AspNetCore.Mvc;
using MeetingRoomReservation.API.DTOs;
using MeetingRoomReservation.API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MeetingRoomReservation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: api/reservations?roomId=1&userName=Ahmet&startDate=2025-02-20&endDate=2025-02-28&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetAllReservations(
            int? roomId,
            string? userName,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1,
            int pageSize = 20)
        {
            var result = await _reservationService.GetAllReservationsAsync(
                roomId, userName, startDate, endDate, page, pageSize);

            return Ok(ApiResponse<object>.SuccessResult(result, "Rezervasyonlar başarıyla getirildi"));
        }

        // GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound(ApiResponse<object>.FailResult("Rezervasyon bulunamadı"));

            return Ok(ApiResponse<object>.SuccessResult(reservation, "Rezervasyon başarıyla getirildi"));
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

            var reservation = await _reservationService.CreateReservationAsync(dto);
            return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id },
                ApiResponse<object>.SuccessResult(reservation, "Rezervasyon başarıyla oluşturuldu"));
        }

        // POST: api/reservations/recurring
        [HttpPost("recurring")]
        public async Task<IActionResult> CreateRecurringReservation([FromBody] CreateRecurringReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

            var reservations = await _reservationService.CreateRecurringReservationAsync(dto);
            return Ok(ApiResponse<object>.SuccessResult(reservations,
                $"{reservations.Count} adet tekrarlayan rezervasyon başarıyla oluşturuldu"));
        }

        // PUT: api/reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] UpdateReservationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

            var reservation = await _reservationService.UpdateReservationAsync(id, dto);

            if (reservation == null)
                return NotFound(ApiResponse<object>.FailResult("Rezervasyon bulunamadı"));

            return Ok(ApiResponse<object>.SuccessResult(reservation, "Rezervasyon başarıyla güncellendi"));
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var result = await _reservationService.CancelReservationAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.FailResult("Rezervasyon bulunamadı"));

            return Ok(ApiResponse<object>.SuccessResult(null, "Rezervasyon başarıyla iptal edildi"));
        }

        // GET: api/reservations/conflicts?roomId=1&start=2025-02-20T10:00:00Z&end=2025-02-20T12:00:00Z
        [HttpGet("conflicts")]
        public async Task<IActionResult> GetConflictingReservations(
            [FromQuery] int roomId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var conflicts = await _reservationService.GetConflictingReservationsAsync(roomId, start, end);
            return Ok(ApiResponse<object>.SuccessResult(conflicts, "Çakışan rezervasyonlar getirildi"));
        }
    }
}