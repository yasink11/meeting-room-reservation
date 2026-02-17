using System;

namespace MeetingRoomReservation.API.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserName { get; set; } // Basit tutuyoruz, auth yok
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantCount { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Tekrarlayan toplantılar için
        public int? RecurringGroupId { get; set; }

        // Navigation Properties
        public virtual Room Room { get; set; }
        public virtual RecurringGroup RecurringGroup { get; set; }
    }
}