using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
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
    public class Initialize(IEnumerable<IDataSeeding> dataSeeding, HotelContext dbContext)
    {
        public IEnumerable<IDataSeeding> DataSeedings { get; set; } = dataSeeding;
        public HotelContext DbContext { get; set; } = dbContext;

        public void Seed()
        {
            DbContext.Database.EnsureDeleted();

            if (!(DbContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                DbContext.Database.Migrate();

                foreach (var seeding in dataSeeding)
                {
                    seeding.SeedData();
                }
                DbContext.SaveChanges();
                Console.WriteLine("Seeding done. Remove this later (INITIALIZE CLASS). Press any key to continue.");
                Console.ReadKey();
            }
            else
            {
                foreach (var seeding in dataSeeding)
                {
                    if (seeding is ReviewSeeding)
                    {
                        seeding.SeedData();
                    }
                }
                Console.WriteLine("No seeding done!!");
                Console.ReadKey();
            }
            var canceledBookings = DbContext.Invoice
                .OrderBy(i => i.DueDate)
                .Include(b => b.Booking)
                .ThenInclude(u => u.User)
                .ThenInclude(u => u.UserInbox)
                .Where(i => !i.IsPaid && i.DueDate < DateTime.Now.AddDays(-10));

            foreach (var invoice in canceledBookings)
            {
                if (invoice.Booking.User.UserName != "Richard")
                {
                    invoice.IsActive = false;
                    invoice.IsPaid = false;
                    invoice.Booking.IsActive = false;
                    var content = "Dear sir/mam, since we have not yet received payment before the due date your booking has been canceled.";
                    SystemMessage.SendSystemMessage(DbContext, invoice.Booking.User, "Your booking has been canceled", content);
                }
                else
                {
                    var content = "Dear sir. We have not yet received payments for your bookings. Due to your status we have made exceptions but this is your last chance.";
                    SystemMessage.SendSystemMessage(DbContext, invoice.Booking.User, "System warning", content);
                }
            }
        }
    }
}
