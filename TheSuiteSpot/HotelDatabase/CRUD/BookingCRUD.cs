using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using static TheSuiteSpot.HotelDatabase.InputHelpers.PrintMessages;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using TheSuiteSpot.HotelDatabase.InputHelpers;
using System.Diagnostics;
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
            var user = CurrentUser.Instance.User;
            if (user.IsAdmin)
            {
                Console.WriteLine("These are the users in our system: ");
                UserCRUD.ReadAllUserNames(ctx);
                var tempU = new UserCRUD(ctx);
                user = tempU.GetUser();
            }

            Console.WriteLine("Do you want a single room?");
            if (ErrorHandling.PromptYesOrNo("Press y to confirm, anything else to deny: "))
            {
                var roomType = GetSuitableRoomType(ctx, 0);
                var availableRooms = ctx.Room.Include(rt => rt.RoomType).Where(rt => rt.RoomType.Id == roomType.Id).ToList();

                var choice = ErrorHandling.MenuValidation(availableRooms, "Choose: ");
                var chosenRoom = availableRooms[choice - 1];
                Create(ctx, chosenRoom, user, 0);
            }
            else
            {
                Console.WriteLine("You have chosen a double room. How many extra beds do you need?");
                var numberOfExtraBeds = (int?)ErrorHandling.AskForValidNumber(0, 2, "TEST");
                if (numberOfExtraBeds == null) { return; }
                var roomType = GetSuitableRoomType(ctx, (int)numberOfExtraBeds);
                var availableRooms = ctx.Room.Include(rt => rt.RoomType).Where(rt => rt.RoomType.Id == roomType.Id).ToList();

                var choice = ErrorHandling.MenuValidation(availableRooms, "Choose: ");
                var chosenRoom = availableRooms[choice - 1];
                Create(ctx, chosenRoom, user, (int)numberOfExtraBeds);
            }
            PressAnyKeyToContinue();
        }
        private static RoomType GetSuitableRoomType(HotelContext ctx, int numberOfExtraBeds)
        {
            return ctx.RoomType.Where(rt => rt.NumberOfExtraBeds == numberOfExtraBeds).First();
        }
        private void Create(HotelContext ctx, Room chosenRoom, User user, int numberOfExtraBeds)
        {
            Console.Write($"Enter a start date, later than {DateTime.Today.ToShortDateString()}: ");
            var chosenDate = DateTime.Parse(Console.ReadLine()).Date + _bookingStart;
            if (chosenDate < DateTime.Now.Date)
            {
                Console.WriteLine($"Pick a date that is later than {DateTime.Now.ToShortDateString()}");
                PressAnyKeyToContinue();
            }
            else
            {

                Console.Clear();
                var allBookingsForRoom = ctx.Booking
                                            .Where(b => b.Room.Id == chosenRoom.Id)
                                            .Where(b => b.EndDate >= chosenDate)
                                            .OrderBy(b => b.StartDate)
                                            .ToList();
                if (allBookingsForRoom.Count == 0)
                {
                    CreateBooking(chosenDate, chosenRoom, user, _maximumBookingDuration, numberOfExtraBeds, ctx);
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
                            Console.WriteLine($"Not available at this date. Changing date to: {chosenDate}");
                        }
                        else if (!(chosenDate >= booking.StartDate && chosenDate <= booking.EndDate))
                        {
                            Console.WriteLine($"Available booking at {chosenDate}");
                            var availableDates = _maximumBookingDuration;
                            if (i != allBookingsForRoom.Count)
                            {
                                var nextBookingDate = allBookingsForRoom[i].StartDate;
                                availableDates = (nextBookingDate - chosenDate).Days;
                            }
                            if (availableDates > 10)
                            {
                                availableDates = _maximumBookingDuration;
                            }
                            Console.WriteLine($"You can book for a maximum of {availableDates} days. (Y/N)");
                            var input = Console.ReadLine();
                            if (input.ToLower() == "y")
                            {
                                CreateBooking(chosenDate, chosenRoom, user, availableDates, numberOfExtraBeds, ctx);
                                break;
                            }
                        }
                        if (i == allBookingsForRoom.Count - 1)
                        {
                            CreateBooking(chosenDate, chosenRoom, user, _maximumBookingDuration, numberOfExtraBeds, ctx);
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
            var numberOfDays = ErrorHandling.ReturnNumberChoice(maxNumberOfDays);
            if (numberOfDays < 0)
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
                booking.HasActiveInvoice = true;

                ctx.Invoice.Add(invoice);
                ctx.SaveChanges();
                SystemMessage.SendBookingConfirmationMessage(ctx, chosenUser, booking);
                SystemMessage.SendInvoiceMessage(ctx, chosenUser, invoice, booking);

                PrintSuccessMessage("A booking has been created with the following information:");
                Console.WriteLine(booking);
                Console.WriteLine(Invoice.GenerateInvoice(invoice));
                PressAnyKeyToContinue();
            }
        }
        public void SoftDelete(HotelContext ctx)
        {
            Console.Clear();
            Console.Write("Search for a booking by its booking id: ");
            var input = Convert.ToInt32(Console.ReadLine());
            var booking = ExactSearch(ctx, input);

            if (booking != null)
            {
                PrintSuccessMessage("The result of your search is: ");
                var info = BookingTemplate(booking);
                Console.WriteLine(info);

                Console.WriteLine("Do you want to delete this booking?");
                if (ErrorHandling.PromptYesOrNo("Press y to confirm, anything else to deny: "))
                {
                    booking.IsActive = false;
                    PrintSuccessMessage("Booking was deleted");
                }
                else
                {
                    PrintErrorMessage("Booking was not deleted.");
                }
            }
            else
            {
                PrintErrorMessage("No booking with that id could be found.");
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
            Console.Clear();
            Console.WriteLine("Enter a booking id: (NOT IMPLEMENTED)");
        }
        public void GeneralSearch(HotelContext ctx)
        {
            Console.Clear();
            Console.Write("Enter a search term to find active bookings that contain that term (try to be specific): ");
            var userInput = ErrorHandling.AskForValidInputString();
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
            //Console.WriteLine("These are the properties you can change for a booking: ");
            //ErrorHandling.MenuValidation(_modelProperties, "Choose an option: ");
            Console.WriteLine("Things to update: dates, room, numberofextrabeds, isactive (this should disable invoice if invoice is in the future, refund the customer if so.");
            Console.Write("Enter start date: ");
            DateTime startDate = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter end date: ");
            DateTime endDate = DateTime.Parse(Console.ReadLine());
            var booking = ctx.Booking.Include(u => u.User).Include(r => r.Room).First();
            bool test = CheckForValidDates(startDate, endDate, booking, ctx);

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
Booking for {booking.User.FirstName} {booking.User.LastName}
Start date: {booking.StartDate}
End date: {booking.EndDate}

Room number: {booking.Room.RoomNumber}
{divider}";
        }
    }
}