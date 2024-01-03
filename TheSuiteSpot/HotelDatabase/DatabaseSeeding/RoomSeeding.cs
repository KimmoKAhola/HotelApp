using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class RoomSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void SeedData()
        {
            if (!DbContext.Room.Any())
            {
                var roomTypes = DbContext.RoomType.ToList();
                var room11 = new Room
                {
                    Description = "A cozy and comfortable suite. For a single person only.",
                    PricePerDay = 2000,
                    RoomNumber = "1001",
                    RoomSize = 24,
                    RoomType = roomTypes[0],
                };

                var room12 = new Room
                {
                    Description = "A cozy and comfortable suite. For a single person only.",
                    PricePerDay = 2000,
                    RoomNumber = "1002",
                    RoomSize = 22,
                    RoomType = roomTypes[0],
                };

                var room21 = new Room
                {
                    Description = "Designed for business travelers or those desiring a stylish retreat. A double room for up to 2 visitors.",
                    PricePerDay = 3500,
                    RoomNumber = "2001",
                    RoomSize = 37,
                    RoomType = roomTypes[1],
                    PricePerExtraBed = 750
                };

                var room22 = new Room
                {
                    Description = "Designed for business travelers or those desiring a stylish retreat. A double room for up to 2 visitors.",
                    PricePerDay = 3800,
                    RoomNumber = "2002",
                    RoomSize = 42,
                    RoomType = roomTypes[1],
                    PricePerExtraBed = 750
                };

                var room31 = new Room
                {
                    Description = "The Deluxe Suite, ideal for guests seeking a touch of indulgence." +
                    " A double room for up to 4 visitors.",
                    PricePerDay = 4300,
                    RoomNumber = "3001",
                    RoomSize = 55,
                    RoomType = roomTypes[2],
                    PricePerExtraBed = 1000
                };

                var room32 = new Room
                {
                    Description = "The Deluxe Suite, ideal for guests seeking a touch of indulgence." +
                    " A double room for up to 4 visitors.",
                    PricePerDay = 4500,
                    RoomNumber = "3002",
                    RoomSize = 60,
                    RoomType = roomTypes[2],
                    PricePerExtraBed = 1000
                };

                var room41 = new Room
                {
                    Description = "The epitome of abundance. The Royal Suite. For a maximum of 6 visitors.",
                    PricePerDay = 9000,
                    RoomNumber = "4001",
                    RoomSize = 100,
                    RoomType = roomTypes[3],
                    PricePerExtraBed = 1200
                };

                var room42 = new Room
                {
                    Description = "The epitome of abundance. The Royal Suite. For a maximum of 6 visitors.",
                    PricePerDay = 9200,
                    RoomNumber = "4002",
                    RoomSize = 110,
                    RoomType = roomTypes[3],
                    PricePerExtraBed = 1400
                };

                DbContext.Room.Add(room11);
                DbContext.Room.Add(room12);
                DbContext.Room.Add(room21);
                DbContext.Room.Add(room22);
                DbContext.Room.Add(room31);
                DbContext.Room.Add(room32);
                DbContext.Room.Add(room41);
                DbContext.Room.Add(room42);
                DbContext.SaveChanges();
            }
        }
    }
}