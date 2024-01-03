using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class BookingSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;
        public void SeedData()
        {
            if (!DbContext.Booking.Any())
            {
                DateTime pastDate = DateTime.Now.Date.AddYears(-2);
                var rooms = DbContext.Room.ToList();
                var allUsers = DbContext.User.Include(u => u.UserInbox).Where(u => !u.IsAdmin && u.UserName != "System").ToList();
                var random = new Random();
                var faker = new Faker();
                for (int j = 0; j < 23; j++)
                {
                    var bookingDate = pastDate.AddMonths(j);
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        var randomUser = random.Next(0, allUsers.Count);
                        var randomRoom = random.Next(0, rooms.Count);
                        var randomDuration = random.Next(1, 10);
                        var booking = new Booking
                        {
                            StartDate = bookingDate.Date + new TimeSpan(8, 0, 0),
                            EndDate = bookingDate.AddDays(randomDuration).Date + new TimeSpan(16, 0, 0),
                            User = allUsers[randomUser],
                            Room = rooms[randomRoom],
                            NumberOfExtraBeds = 0
                        };
                        DbContext.Booking.Add(booking);
                        DbContext.SaveChanges();
                        SystemMessage.SendBookingConfirmationMessage(DbContext, booking.User, booking);
                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    var randomUser = random.Next(0, allUsers.Count);
                    var randomRoom = random.Next(0, rooms.Count);
                    var randomDuration = random.Next(1, 10);
                    var startDate = DateTime.Today.Date.AddDays(randomDuration);
                    var booking = new Booking
                    {
                        StartDate = startDate + new TimeSpan(8, 0, 0),
                        EndDate = startDate.AddDays(randomDuration).Date + new TimeSpan(16, 0, 0),
                        User = allUsers[randomUser],
                        Room = rooms[randomRoom],
                        NumberOfExtraBeds = 0
                    };
                    DbContext.Booking.Add(booking);
                    DbContext.SaveChanges();
                    SystemMessage.SendBookingConfirmationMessage(DbContext, booking.User, booking);
                }
            }
        }
    }
}