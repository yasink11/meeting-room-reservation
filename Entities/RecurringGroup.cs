using System;
using System.Collections.Generic;

namespace MeetingRoomReservation.API.Entities
{
    public class RecurringGroup
    {
        public int Id { get; set; }
        public string Pattern { get; set; } // "Weekly", "Daily", "Monthly"
        public int Interval { get; set; } // Her X haftada bir (1 = her hafta)
        public string DayOfWeek { get; set; } // "Monday", "Tuesday", vb.
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ExceptionDates { get; set; } // Virgülle ayrılmış tarihler (tatil günleri)
        public DateTime CreatedDate { get; set; }

        // Navigation Property
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}