using System;

namespace MeetingRoomReservation.API.Entities
{
    public class RecurringGroupExceptionDate
    {
        public int Id { get; set; }

        public int RecurringGroupId { get; set; }
        public DateTime ExceptionDate { get; set; } 

        public RecurringGroup RecurringGroup { get; set; }
    }
}
