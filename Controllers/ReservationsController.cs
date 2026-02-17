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

        // GET: api/reservations
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationService.GetAllReservationsAsync();
                return Ok(ApiResponse<object>.SuccessResult(reservations, "Rezervasyonlar başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }

        // GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationById(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);

                if (reservation == null)
                    return NotFound(ApiResponse<object>.FailResult("Rezervasyon bulunamadı"));

                return Ok(ApiResponse<object>.SuccessResult(reservation, "Rezervasyon başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

                var reservation = await _reservationService.CreateReservationAsync(dto);
                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.Id }, ApiResponse<object>.SuccessResult(reservation, "Rezervasyon başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResult(ex.Message));
            }
        }

        // POST: api/reservations/recurring
        [HttpPost("recurring")]
        public async Task<IActionResult> CreateRecurringReservation([FromBody] CreateRecurringReservationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

                var reservations = await _reservationService.CreateRecurringReservationAsync(dto);
                return Ok(ApiResponse<object>.SuccessResult(reservations, $"{reservations.Count} adet tekrarlayan rezervasyon başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResult(ex.Message));
            }
        }

        // PUT: api/reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] UpdateReservationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<object>.FailResult("Geçersiz veri"));

                var reservation = await _reservationService.UpdateReservationAsync(id, dto);

                if (reservation == null)
                    return NotFound(ApiResponse<object>.FailResult("Rezervasyon bulunamadı"));

                return Ok(ApiResponse<object>.SuccessResult(reservation, "Rezervasyon başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResult(ex.Message));
            }
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(id);

                if (!result)
                    return NotFound(ApiResponse<object>.FailResult("Rezervasyon bulunamadı"));

                return Ok(ApiResponse<object>.SuccessResult(null, "Rezervasyon başarıyla iptal edildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResult(ex.Message));
            }
        }

        // GET: api/reservations/conflicts?roomId=1&start=2025-02-20T10:00:00&end=2025-02-20T12:00:00
        [HttpGet("conflicts")]
        public async Task<IActionResult> GetConflictingReservations([FromQuery] int roomId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var conflicts = await _reservationService.GetConflictingReservationsAsync(roomId, start, end);
                return Ok(ApiResponse<object>.SuccessResult(conflicts, "Çakışan rezervasyonlar getirildi"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResult("Bir hata oluştu", new System.Collections.Generic.List<string> { ex.Message }));
            }
        }
    }
}