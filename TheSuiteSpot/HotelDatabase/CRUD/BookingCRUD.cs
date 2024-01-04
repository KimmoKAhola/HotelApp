﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using System.Diagnostics;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;
namespace TheSuiteSpot.HotelDatabase.CRUD
{
    public class BookingCRUD(HotelContext dbContext) : ICRUD
    {
        public HotelContext DbContext { get; set; } = dbContext;
        private List<IMenu> _menus;
        private readonly int _maximumBookingDuration = 10; // Business rule
        private readonly TimeSpan _bookingStart = new TimeSpan(8, 0, 0); // business rule
        private Dictionary<int, string> _modelProperties = new Dictionary<int, string>()
        {
            {1, "Start Date"},
            {2, "End Date"},
            {3, "Room"},
            {4, "Number of beds"},

        };
        public void Create(HotelContext ctx)
        {
            Console.Clear();
            var user = CurrentUser.Instance.User;
            if (user.IsAdmin)
            {
                Console.WriteLine("These are the users in our system: ");
                UserCRUD.ReadAllUserNames(ctx);
                var tempU = new UserCRUD(ctx);
                user = tempU.GetUser();
                if (user == null)
                {
                    PrintErrorMessage("Returning to the main menu.");
                    PressAnyKeyToContinue();
                    return;
                }
            }

            PrintNotification("Do you want a single room?");
            if (UserInputValidation.PromptYesOrNo("Press y to confirm, anything else to pick a double room: "))
            {
                PrintNotification("You chose a single room.");
                var roomType = GetSuitableRoomType(ctx, 0);

                var availableRooms = ctx.Room
                    .Include(rt => rt.RoomType)
                    .Where(rt => rt.RoomType.Id == roomType.Id)
                    .ToList();
                Console.WriteLine();
                var choice = UserInputValidation.MenuValidation(availableRooms, "These are the available rooms\n");
                var chosenRoom = availableRooms[choice - 1];
                Console.Clear();
                PrintNotification($"You chose this room: ");
                Console.WriteLine(chosenRoom);
                Create(ctx, chosenRoom, user, 0);
            }
            else
            {
                PrintNotification("You chose a double room.");
                var numberOfExtraBeds = (int?)UserInputValidation.AskForValidNumber(0, 2, "How many extra beds do you want: ");
                if (numberOfExtraBeds == null) { return; }
                var roomType = GetSuitableRoomType(ctx, (int)numberOfExtraBeds);

                var availableRooms = ctx.Room
                    .Include(rt => rt.RoomType)
                    .Where(rt => rt.RoomType.Id == roomType.Id)
                    .ToList();

                var choice = UserInputValidation.MenuValidation(availableRooms, "Choose: ");
                var chosenRoom = availableRooms[choice - 1];
                Create(ctx, chosenRoom, user, (int)numberOfExtraBeds);
            }
            PressAnyKeyToContinue();
        }
        private bool CheckForVoucher(User guest)
        {
            var result = from message in DbContext.Message
                         where message.Voucher != null
                         join voucher in DbContext.Voucher on message.Voucher.Id equals voucher.Id
                         join userInbox in DbContext.UserInbox on message.UserInbox.Id equals userInbox.Id
                         join user in DbContext.User on userInbox.Id equals user.UserInbox.Id
                         where user.Id == guest.Id
                         select new
                         {
                             Message = message,
                             Voucher = voucher,
                             UserInbox = userInbox,
                             User = user
                         };

            var resultList = result.ToList();

            return true;
        }
        private static RoomType GetSuitableRoomType(HotelContext ctx, int numberOfExtraBeds)
        {
            return ctx.RoomType.Where(rt => rt.NumberOfExtraBeds == numberOfExtraBeds).First();
        }
        private void Create(HotelContext ctx, Room chosenRoom, User user, int numberOfExtraBeds)
        {
            var chosenDate = UserInputValidation.AskForValidDate(DateTime.Now);
            if (chosenDate == null) { return; }
            else
            {
                chosenDate = (DateTime)chosenDate;
                Console.Clear();
                var allBookingsForRoom = ctx.Booking
                                            .Where(b => b.Room.Id == chosenRoom.Id)
                                            .Where(b => b.EndDate >= chosenDate)
                                            .OrderBy(b => b.StartDate)
                                            .ToList();
                if (allBookingsForRoom.Count == 0)
                {
                    CreateBooking((DateTime)chosenDate, chosenRoom, user, _maximumBookingDuration, numberOfExtraBeds, ctx);
                }
                else
                {
                    for (int i = 0; i < allBookingsForRoom.Count; i++)
                    {
                        var originalDate = chosenDate;
                        var booking = allBookingsForRoom[i];

                        if (chosenDate >= booking.StartDate && chosenDate <= booking.EndDate)
                        {
                            chosenDate = booking.EndDate.Date.AddDays(1) + _bookingStart;
                        }
                        else if (!(chosenDate >= booking.StartDate && chosenDate <= booking.EndDate))
                        {
                            PrintNotification($"\nYour chosen date was not available. The first available booking is at {chosenDate}\n");
                            var availableDates = _maximumBookingDuration;
                            if (i != allBookingsForRoom.Count)
                            {
                                var nextBookingDate = allBookingsForRoom[i].StartDate;
                                availableDates = (nextBookingDate - (DateTime)chosenDate).Days;
                            }
                            if (availableDates > 10)
                            {
                                availableDates = _maximumBookingDuration;
                            }
                            if (UserInputValidation.PromptYesOrNo($"You can book for a maximum of {availableDates} days. Y/N?"))
                            {
                                CreateBooking((DateTime)chosenDate, chosenRoom, user, availableDates, numberOfExtraBeds, ctx);
                                break;
                            }
                            else
                            {
                                PrintNotification($"You declined this booking");
                                break;
                            }
                        }
                        if (i == allBookingsForRoom.Count - 1)
                        {
                            if (chosenDate != originalDate)
                            {
                                PrintNotification($"Your chosen date was not available. The first available time is at {chosenDate}");
                            }
                            CreateBooking((DateTime)chosenDate, chosenRoom, user, _maximumBookingDuration, numberOfExtraBeds, ctx);
                            break;
                        }
                    }
                }
            }
        }
        private void CreateBooking(DateTime newDate, Room chosenRoom, User chosenUser, int maxNumberOfDays, int numberOfExtraBeds, HotelContext ctx)
        {

            Console.WriteLine($"The maximum amount of days you can book in a row is: {maxNumberOfDays}");
            Console.Write("Enter the amount of days you want to book: ");
            var numberOfDays = UserInputValidation.ReturnNumberChoice(maxNumberOfDays);
            if (numberOfDays == -1)
            {
                PrintErrorMessage("User chose to exit. Returning to main menu");
                PressAnyKeyToContinue();
                _ = new MainMenu(_menus);
            }
            else
            {
                ctx.SaveChanges();
                chosenUser = ctx.User.Where(u => u.Id == chosenUser.Id).Include(u => u.UserInbox).First();
                var booking = new Booking
                {
                    StartDate = newDate,
                    EndDate = newDate.AddDays(numberOfDays),
                    User = chosenUser,
                    Room = chosenRoom,
                    NumberOfExtraBeds = numberOfExtraBeds
                };
                ctx.SaveChanges();
                ctx.Booking.Add(booking);
                var totalDays = (int)(booking.EndDate - booking.StartDate).Days;

                var invoice = new Invoice
                {
                    DateCreated = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(10),
                    Amount = totalDays * booking.Room.PricePerDay,
                    Booking = booking,
                };
                if (booking.NumberOfExtraBeds > 0)
                {
                    invoice.Amount = Invoice.CalculateAdditionalCost(invoice);
                }

                ctx.Invoice.Add(invoice);
                ctx.SaveChanges();
                SystemMessage.SendBookingConfirmationMessage(ctx, chosenUser, booking);
                SystemMessage.SendInvoiceMessage(ctx, chosenUser, invoice, booking);
                Console.Clear();
                PrintSuccessMessage("A booking has been created with the following information: ");
                var info = BookingTemplate(booking);
                Console.WriteLine(info + "\n");
                PrintSuccessMessage("An invoice has been created and sent to the chosen guest: ");
                Console.WriteLine(Invoice.GenerateInvoice(invoice));
            }
        }
        public void SoftDelete(HotelContext ctx)
        {
            Console.Clear();

            var futureBookings = ctx.Booking
                .OrderBy(b => b.StartDate)
                .Where(b => b.StartDate > DateTime.Now && b.IsActive)
                .Include(r => r.Room)
                .Join(ctx.Invoice, b => b.Id, i => i.Id, (b, i) => new { Booking = b, Invoice = i })
                .Join(ctx.User, b => b.Booking.User.Id, u => u.Id, (b, u) => new { Booking = b, User = u })
                .Join(ctx.UserInbox, u => u.User.UserInbox.Id, ui => ui.Id, (u, ui) => new { User = u, UserInbox = ui })
                .ToList();
            if (futureBookings.Count == 0)
            {
                PrintNotification("There are no future bookings");
            }
            else
            {
                List<int> bookingIds = new List<int>();
                foreach (var booking in futureBookings)
                {
                    bookingIds.Add(booking.User.Booking.Booking.Id);
                    var info = BookingTemplate(booking.User.Booking.Booking);
                    Console.WriteLine(info);
                }
                var choice = UserInputValidation.MenuValidation(bookingIds, "\nPick one of the available booking Ids to delete it.\n");
                if (choice == -1) { return; }
                var chosenId = bookingIds[choice - 1];
                var chosenBooking = futureBookings.Where(b => b.User.Booking.Booking.Id == chosenId).First();
                PrintNotification($"You chose option {choice} with booking Id {bookingIds[choice - 1]}");
                if (UserInputValidation.PromptYesOrNo("Press y to delete it, anything else to skip: "))
                {
                    PrintNotification("You chose to delete the booking");
                    chosenBooking.User.Booking.Booking.IsActive = false;
                    chosenBooking.User.Booking.Invoice.IsActive = false;
                    ctx.SaveChanges();
                    if (chosenBooking.User.Booking.Invoice.IsPaid)
                    {
                        var content = "Dear sir/mam. Your booking has been canceled and your payment has been fully refunded.";
                        SystemMessage.SendSystemMessage(ctx, chosenBooking.User.User, "Refund", content);
                    }
                    else
                    {
                        var content = "Dear sir/mam. Your booking has been canceled.";
                        SystemMessage.SendSystemMessage(ctx, chosenBooking.User.User, "Cancellation confirmed", content);
                    }
                }
                PrintSuccessMessage("\nThe booking has been canceled and the customer has been notified.\nAny related invoice has been canceled.");
            }
            PressAnyKeyToContinue();
        }
        public void ReadAll(HotelContext ctx)
        {
            Console.Clear();
            Console.WriteLine("These are all active (current and future) bookings: ");

            var allBookings = ctx.Booking
                .Where(b => b.StartDate < DateTime.Today || b.EndDate > DateTime.Today)
                .OrderBy(b => b.StartDate)
                .Include(u => u.User)
                .Include(r => r.Room);

            if (allBookings.Count() > 10)
            {
                PrintNotification("Too many active bookings. The result has been limited to 10.");
                var shortList = allBookings.Take(10).ToList();
                foreach (var booking in shortList)
                {
                    var info = BookingTemplate(booking);
                    Console.WriteLine(info);
                }
            }
            else
            {

                foreach (var booking in allBookings)
                {
                    var info = BookingTemplate(booking);
                    Console.WriteLine(info);
                }
            }
            PressAnyKeyToContinue();
        }
        public void ExactSearch(HotelContext ctx)
        {
            var user = ctx.User.Where(u => !u.IsAdmin && u.Id == 29).First();
            CheckForVoucher(user);
        }

        public void CreateManually()
        {
            Console.Clear();
            var chosenStartDate = UserInputValidation.AskForValidDate(DateTime.Now);
            if (chosenStartDate == null) { return; }
            DateTime? chosenEndDate = DateTime.MinValue;
            while (true)
            {
                chosenEndDate = UserInputValidation.AskForValidDate(chosenStartDate.Value.AddDays(1));
                if (chosenEndDate < chosenStartDate.Value.AddDays(10))
                {
                    Console.WriteLine(chosenStartDate);
                    Console.WriteLine(chosenEndDate);
                    break;
                }
                else
                {
                    PrintErrorMessage($"You need to enter an end date at most 10 days after {chosenStartDate.Value.ToShortDateString()}");
                }
                if (chosenEndDate == null) { return; }
            }

            var availableRooms = DbContext.Room
                    .Where(room => !room.Bookings
                    .Any(b =>
        ((chosenStartDate >= b.StartDate && chosenStartDate <= b.EndDate)
                    || (chosenEndDate >= b.StartDate && chosenEndDate <= b.EndDate)
                    || (chosenStartDate <= b.StartDate && chosenEndDate >= b.EndDate))))
                    .ToList();

            if (availableRooms.Count > 0)
            {
                List<int> roomIds = new List<int>();
                PrintNotification("These are our available rooms at those given dates: ");
                foreach (var item in availableRooms)
                {
                    roomIds.Add(item.Id);
                    Console.WriteLine($"Room id: {item.Id} - Room number: {item.RoomNumber}");
                }
                UserInputValidation.MenuValidation(roomIds, "\n");
            }
            else
            {
                PrintErrorMessage("No rooms are available between your given dates.");
            }

            PressAnyKeyToContinue();
        }
        public void GeneralSearch(HotelContext ctx)
        {
            Console.Clear();
            Console.Write("Enter a search term to find active bookings that contain that term (try to be specific): ");
            var userInput = UserInputValidation.AskForValidInputString();
            if (userInput == null) { return; }
            var bookings = ctx.Booking
                .Where(b => b.IsActive)
                .OrderBy(b => b.StartDate)
                .Include(r => r.Room)
                .Include(u => u.User)
                .Where(b => b.User.UserName.ToLower().Contains(userInput)
                || b.User.Email.ToLower().Contains(userInput)
                || b.User.FirstName.ToLower().Contains(userInput)
                || b.User.LastName.ToLower().Contains(userInput));

            if (bookings.Count() > 10)
            {
                PrintNotification($"Your search term of \"{userInput}\" yielded more than 10 results. Only showing the top 10 results, ordered by booking start date.");
                bookings.Take(10);
            }
            foreach (var booking in bookings)
            {
                var info = FormatBooking(booking);
                Console.WriteLine(info);
            }
            PressAnyKeyToContinue();
        }
        public static string FormatBooking(Booking booking)
        {
            var header = new string('-', booking.EndDate.ToString().Length + booking.StartDate.ToString().Length + 22);
            var result = $"{header}" +
                $"\nBooking for the user with username: " +
                $"{booking.User.UserName}" +
                $"\nBooking dates: [{booking.StartDate}] - [{booking.EndDate}]" +
                $"\nRoom number: {booking.Room.RoomNumber}" +
                $"\nPrice per day: {booking.Room.PricePerDay}\n";

            return result;
        }
        private static Booking ExactSearch(HotelContext ctx, int bookingId)
        {
            var exactBooking = ctx.Booking
                .Where(b => b.Id == bookingId)
                .Include(r => r.Room)
                .Include(u => u.User)
                .FirstOrDefault();

            return exactBooking;
        }
        public void Update(HotelContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Search for a user to update his/her future booking.");
            Console.WriteLine("Can only update number of beds and start stop date within the time window, ie move start date forward or end date back");

            PressAnyKeyToContinue();
        }
        public static bool CheckIfUserHasBookings(User user, HotelContext ctx)
        {
            var userResult = ctx.User
                .Where(u => u.Id == user.Id)
                .Include(b => b.Bookings.Where(b => b.EndDate > DateTime.Today))
                .First();

            if (userResult.Bookings.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIfUserHasVoucher(User user, HotelContext ctx)
        {
            var test = ctx.User.Where(u => u.UserName == user.UserName)
                .Include(u => u.UserInbox)
                .ThenInclude(m => m.Messages)
                .ThenInclude(v => v.Voucher)
                .ToList();
            return true;
        }
        public static bool CheckForValidDates(DateTime startDate, DateTime endDate, Booking booking, HotelContext ctx)
        {
            var conflictingBookings = ctx.Booking
                    .Include(b => b.Room)
                    .Where(b => b.Room.Id == booking.Room.Id
                    && b.Id != booking.Id
                    && ((startDate >= b.StartDate && startDate <= b.EndDate)
                    || (endDate >= b.StartDate && endDate <= b.EndDate)
                    || (startDate <= b.StartDate && endDate >= b.EndDate)))
                    .ToList();

            return conflictingBookings.Count == 0;
        }
        private static string BookingTemplate(Booking booking)
        {
            var divider = new string('-', booking.StartDate.ToString().Length + 12);
            var nameDivider = new string('-', booking.StartDate.ToString().Length + 12);
            if (nameDivider.Length > divider.Length)
            {
                divider = nameDivider;
            }

            return $@"
{divider}
Booking Id: {booking.Id}
Booking for {booking.User.FirstName} {booking.User.LastName}
Start date: {booking.StartDate}
End date: {booking.EndDate}

Room number: {booking.Room.RoomNumber}
{divider}";
        }
    }
}