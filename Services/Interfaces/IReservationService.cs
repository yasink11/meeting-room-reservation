using MeetingRoomReservation.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomReservation.API.Services.Interfaces
{
    public interface IReservationService
    {
        Task<PagedResult<ReservationDto>> GetAllReservationsAsync(
            int? roomId = null,
            string userName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20);
        Task<ReservationDto> GetReservationByIdAsync(int id);
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<List<ReservationDto>> CreateRecurringReservationAsync(CreateRecurringReservationDto dto);
        Task<ReservationDto> UpdateReservationAsync(int id, UpdateReservationDto dto);
        Task<bool> CancelReservationAsync(int id);
        Task<List<ReservationDto>> GetConflictingReservationsAsync(int roomId, DateTime start, DateTime end);
    }
}