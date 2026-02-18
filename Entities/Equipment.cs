namespace MeetingRoomReservation.API.Entities
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Projeksiyon", "Mikrofon" vb.

        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}