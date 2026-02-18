namespace MeetingRoomReservation.API.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Floor { get; set; }

        public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}