using System;
using System.Collections.Generic;

namespace MeetingRoomReservation.API.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Floor { get; set; }
        public List<string> Equipments { get; set; }
    }

    public class CreateRoomDto
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Floor { get; set; }
        public List<string> Equipments { get; set; }
    }

    public class UpdateRoomDto
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Floor { get; set; }
        public List<string> Equipments { get; set; }
    }
}