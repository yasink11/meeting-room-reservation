using System;
using System.Collections.Generic;

namespace MeetingRoomReservation.API.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Floor { get; set; }
        public string Equipment { get; set; } // Virgülle ayrılmış: "Projeksiyon,Beyaz Tahta,Telefon"
        public bool IsActive { get; set; } // Soft delete için
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation Property
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}