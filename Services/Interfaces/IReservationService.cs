using MeetingRoomReservation.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRoomReservation.API.Services.Interfaces
{
    public interface IReservationService
    {
        Task<List<ReservationDto>> GetAllReservationsAsync();
        Task<ReservationDto> GetReservationByIdAsync(int id);
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<List<ReservationDto>> CreateRecurringReservationAsync(CreateRecurringReservationDto dto);
        Task<ReservationDto> UpdateReservationAsync(int id, UpdateReservationDto dto);
        Task<bool> CancelReservationAsync(int id);
        Task<List<ReservationDto>> GetConflictingReservationsAsync(int roomId, DateTime start, DateTime end);
    }
}