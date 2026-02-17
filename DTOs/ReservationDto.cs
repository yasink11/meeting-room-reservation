using System;

namespace MeetingRoomReservation.API.DTOs
{
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
        public string Pattern { get; set; } // "Weekly"
        public int Interval { get; set; } // 1 = her hafta
        public string DayOfWeek { get; set; } // "Monday"
        public int WeekCount { get; set; } // Kaç hafta tekrarlanacak
        public string ExceptionDates { get; set; } // "2025-03-10,2025-04-15"
    }
}