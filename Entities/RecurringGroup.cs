using System;
using System.Collections.Generic;

namespace MeetingRoomReservation.API.Entities
{
    public class RecurringGroup
    {
        public int Id { get; set; }
        public string Pattern { get; set; }    // "Weekly", "Daily", "Monthly"
        public int Interval { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<RecurringGroupExceptionDate> ExceptionDates { get; set; }
            = new List<RecurringGroupExceptionDate>();
    }
}
