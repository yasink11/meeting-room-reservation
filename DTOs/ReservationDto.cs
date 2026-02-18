using System;

namespace MeetingRoomReservation.API.DTOs
{
    // UTC olarak döner — JSON'da "Z" suffix'i gelir (örn: "2025-03-10T10:00:00Z")
    public class ReservationDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantCount { get; set; }
        public int? RecurringGroupId { get; set; }
    }

    // Client UTC gönderir (örn: "2025-03-10T10:00:00Z")
    // Göndermezse ToUniversalTime() ile normalize edilir
    public class CreateReservationDto
    {
        public int RoomId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantCount { get; set; }
    }

    public class UpdateReservationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantCount { get; set; }
    }

    public class CreateRecurringReservationDto
    {
        public int RoomId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantCount { get; set; }

        public string Pattern { get; set; }
        public int Interval { get; set; }
        public string DayOfWeek { get; set; }
        public int WeekCount { get; set; }

        public List<DateTime> ExceptionDates { get; set; } = new();
    }
}