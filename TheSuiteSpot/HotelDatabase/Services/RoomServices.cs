using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;

namespace TheSuiteSpot.HotelDatabase.Services
{
    public class RoomServices(HotelContext dbContext)
    {
        public HotelContext DbContext { get; set; } = dbContext;
        public List<RoomType>? GetSuitableRoomType(int numberOfGuests)
        {
            var roomTypes = DbContext
                .RoomType
                .OrderBy(rt => rt.NumberOfExtraBeds)
                .ToList();
            if (numberOfGuests > 0 && numberOfGuests <= 1)
            {
                return roomTypes; // all room types are suitable
            }
            else if (numberOfGuests > 1 && numberOfGuests <= 2)
            {
                return roomTypes.TakeLast(3).ToList(); // the last 3 are suitable
            }
            else if (numberOfGuests > 2 && numberOfGuests <= 4)
            {
                return roomTypes.TakeLast(2).ToList(); // the last 2 are suitable
            }
            else
            {
                return roomTypes.TakeLast(1).ToList(); // Only the last is suitable
            }
        }
    }
}
