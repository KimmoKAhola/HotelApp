using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Models;

namespace DatabaseLibrary
{
    public static class RoomTemplateUtility
    {
        public static string RoomTemplate(Room room, string header)
        {
            return $@"{header}
{room.Description}

Room number : {room.RoomNumber} - size: {room.RoomSize} m²
Price per day: {room.PricePerDay:C2}
Number of extra beds: {room.RoomType.NumberOfExtraBeds}
Price per extra bed: {room.PricePerExtraBed:C2} per day 
";
        }
    }
}
