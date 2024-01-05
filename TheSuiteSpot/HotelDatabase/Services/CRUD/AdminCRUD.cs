using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;
using TheSuiteSpot.HotelDatabase.Services;

namespace TheSuiteSpot.HotelDatabase.Services.CRUD
{
    public class AdminCRUD(HotelContext dbContext)
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void Create(HotelContext ctx)
        {
            Console.Clear();
            var userAverageSpending = DbContext.Invoice
                .Where(i => i.IsPaid)
                .GroupBy(i => i.Booking.User.Id)
                .Select(c => new
                {
                    UserId = c.Key,
                    AverageSpending = c.Average(i => i.Amount)
                }).Average(u => u.AverageSpending);

            var bigSpenders = DbContext.Invoice
                            .Where(i => i.IsPaid)
                            .Include(b => b.Booking)
                            .ThenInclude(u => u.User)
                            .ThenInclude(u => u.UserInbox)
                            .GroupBy(u => u.Booking.User)
                            .Select(c => new
                            {
                                User = c.Key,
                                AverageSpending = c.Average(i => i.Amount)
                            })
                            .Where(user => user.AverageSpending >=
                                DbContext.Invoice
                                    .Where(i => i.IsPaid)
                                    .GroupBy(i => i.Booking.User)
                                    .Select(c => c.Average(i => i.Amount))
                                    .Average())
                            .ToList()
                            .OrderByDescending(x => x.AverageSpending);
            PrintNotification($"Here are our customer that has spent more than the average of {userAverageSpending:C2}");
            foreach (var bigSpender in bigSpenders)
            {
                var divider = new string('-', 40);
                Console.WriteLine(divider);
                Console.WriteLine($"Name: {bigSpender.User.FirstName} {bigSpender.User.LastName}");
                Console.WriteLine($"Username {bigSpender.User.UserName}");
                Console.WriteLine($"Has spent: {bigSpender.AverageSpending:C2}");
                if (bigSpender == bigSpenders.Last())
                {
                    Console.WriteLine(divider);
                    Console.WriteLine();
                }
            }
            if (UserInputValidation.PromptYesOrNo("Do you want to reward them with a voucher?\nPress y to confirm, anything else to exit: "))
            {
                PrintNotification("You chose yes\n");
                var discount = UserInputValidation.AskForValidNumber(1, 99, "Choose a discount percentage. ");
                if (discount == null) { return; }
                foreach (var bigSpender in bigSpenders)
                {
                    SystemMessageServices.SendRewardMessage(DbContext, (decimal)discount, bigSpender.User);
                }
                PrintSuccessMessage("Vouchers have been sent.");
            }
            DbContext.SaveChanges();
        }

        public void CreateSystemMessage(HotelContext ctx)
        {
            Console.Clear();
            var allUsers = DbContext.User
                .Include(u => u.UserInbox)
                .Where(u => u.IsActive && !u.IsAdmin && u.UserName != CurrentUser.Instance.User.UserName);
            string topic = UserInputValidation.AskForValidInputString("message topic");
            string content = UserInputValidation.AskForValidInputString("message content");
            foreach (var user in allUsers)
            {
                SystemMessageServices.SendSystemMessage(DbContext, user, topic, content);
            }
            DbContext.SaveChanges();
            PrintNotification("A system message has been sent to all users.");
        }

        public void ViewMostPopularRooms(HotelContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Crunching numbers, this might take a few seconds...");

            var roomsByPopularity = DbContext.Booking
                .Include(r => r.Room)
                .ThenInclude(r => r.Reviews)
                .AsEnumerable()
                .GroupBy(r => r.Room)
                .Select(c => new
                {
                    Number = c.Key.RoomNumber,
                    ReviewScore = c.Key.Reviews,
                    NumberOfBookings = c.Count(),
                }).OrderByDescending(c => c.NumberOfBookings).ToList();

            Console.Clear();
            PrintNotification("These are our most popular rooms: ");
            foreach (var room in roomsByPopularity)
            {
                double averageScore = Math.Round(room.ReviewScore.Average(r => (double)r.StarsGiven), 1);
                var header = new string('-', $"Number of bookings: {room.NumberOfBookings} - Room: {room.Number} - average review score: {averageScore}".Length);
                Console.WriteLine(header);
                PrintNotification($"Room: {room.Number} - Number of bookings: {room.NumberOfBookings} - average review score: {averageScore}");
            }
            Console.WriteLine();
        }
        public void ViewHighestSpenders(HotelContext ctx)
        {
            Console.Clear();
            PrintSuccessMessage("This displays the highest spenders at our hotel - ie the customers with paid invoices.");
            var usersSortedBySpending = DbContext.Invoice
                .Where(i => i.IsPaid)
                .Include(b => b.Booking)
                .ThenInclude(u => u.User)
                .GroupBy(b => b.Booking.User)
                .Select(c => new
                {
                    User = c.Key,
                    Sum = c.Sum(i => i.Amount),
                }).OrderByDescending(r => r.Sum);

            foreach (var user in usersSortedBySpending)
            {
                var divider = new string('-', $"Username: {user.User.UserName} - sum spent: {user.Sum:C2}".Length);
                PrintNotification($"Username: {user.User.UserName} - sum spent: {user.Sum:C2}");
                Console.WriteLine(divider);
            }
        }

        public void SendVouchers()
        {
            Console.Clear();
            var allSubsribers = DbContext.User
                .Include(ui => ui.UserInbox)
                .Where(u => !u.IsAdmin && u.IsSubscriber && u.IsActive);

            if (UserInputValidation.PromptYesOrNo("Press y to send vouchers, anything else to exit: "))
            {
                var discount = UserInputValidation.AskForValidNumber(1, 99, "Choose a discount percentage. ");
                if (discount == null) { return; }
                foreach (var subscriber in allSubsribers)
                {
                    SystemMessageServices.SendRewardMessage(DbContext, (decimal)discount, subscriber);
                }
                PrintSuccessMessage("Vouchers have been sent to our subsribers.");
            }
            PressAnyKeyToContinue();
        }
    }
}
