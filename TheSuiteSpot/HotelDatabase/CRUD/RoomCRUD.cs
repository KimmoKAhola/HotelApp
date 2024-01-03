using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;

namespace TheSuiteSpot.HotelDatabase.CRUD
{
    public class RoomCRUD(HotelContext dbContext) : ICRUD
    {
        public HotelContext DbContext { get; set; } = dbContext;
        private Dictionary<int, string> _modelProperties = new Dictionary<int, string>()
        {
            {1, "Description."},
            {2, "Room Number."},
            {3, "Room Size."},
            {4, "Room Price."},
            {5, "Room Type" },
            {6, "Room booking availability."},
        };
        private Dictionary<int, string> _roomTypeProperties = new Dictionary<int, string>()
        {
            {1, "Suite name." },
            {2, "Number of extra beds." },
            {3, "Double or single room." },
        };
        public void Create(HotelContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Create a new hotel room.");
            var roomTypes = ctx.RoomType.ToList();
            var input = UserInputValidation.MenuValidation(roomTypes, "Choose a suite type: ");
            Console.Write("Enter a unique room number: ");
            string? roomNumber;
            while (true)
            {
                roomNumber = UserInputValidation.AskForValidRoomNumber("room number", 4, 6);
                if (!ctx.Room.Any(r => r.RoomNumber == roomNumber))
                {
                    break;
                }
                PrintErrorMessage("Room number is already taken.");
            }
            if (roomNumber == null) { return; }
            Console.Write("Enter a room size: ");
            var roomSize = (int?)UserInputValidation.AskForValidNumber(20, 200, "Enter a number between 1 and {maximumInput}, or press 'e' to exit: ");
            if (roomSize == null) { return; }
            var room = new Room
            {
                Description = "Test",
                RoomType = roomTypes[input - 1],
                RoomNumber = roomNumber,
                RoomSize = (int)roomSize
            };
            Console.Write("Enter a price per day for the room: ");
            var pricePerDay = UserInputValidation.AskForValidNumber(1000, 20000, "temp");
            if (pricePerDay == null) { return; }
            room.PricePerDay = (decimal)pricePerDay;
            ctx.Add(room);
            ctx.SaveChanges();

            PrintSuccessMessage("Room has been added successfully.");
            Console.WriteLine(room);
            PressAnyKeyToContinue();
        }
        public void ExactSearch(HotelContext ctx)
        {
            Console.Clear();
            var allRooms = ctx.Room;
            Console.Write("Enter a room number to search for: ");
            var input = UserInputValidation.AskForValidInputString();

            var searchResult = ctx.Room.Include(rt => rt.RoomType).Where(r => r.RoomNumber.Equals(input)).FirstOrDefault();
            if (searchResult == null)
            {
                PrintErrorMessage($"No room with room number \"{input}\" was found.");
            }
            else
            {
                PrintSuccessMessage("Room found:");
                Console.WriteLine(searchResult);
            }
            PressAnyKeyToContinue();
        }
        public void ReadAll(HotelContext ctx)
        {
            Console.Clear();
            PrintAllAvailableRooms(ctx);
            PressAnyKeyToContinue();
        }
        public void Update(HotelContext ctx)
        {
            Console.Clear();
            var allRooms = ctx.Room.Include(rt => rt.RoomType);

            Console.WriteLine("These are the available rooms in the hotel: ");
            foreach (var room in allRooms)
            {
                Console.WriteLine($"Room {room.RoomNumber} - {room.RoomType}");
            }
            Console.Write("Enter a room number to update that room: ");
            var roomNumber = UserInputValidation.AskForValidInputString();
            if (roomNumber == null) { return; }
            var chosenRoom = GetRoom(roomNumber, ctx);
            if (chosenRoom != null)
            {
                Console.Clear();
                Console.WriteLine($"You wish to update this room: {chosenRoom}\n");
                Console.WriteLine("These are the properties you can change: ");
                UserInputValidation.MenuValidation(_modelProperties, "Choose an option: ");
            }
            else
            {
                PrintErrorMessage($"No room with the room number \"{roomNumber}\" exists");
            }
            PressAnyKeyToContinue();
        }
        public void UpdateRoomType(HotelContext ctx)
        {
            Console.Clear();
            var allRoomTypes = ctx.RoomType.ToList();
            Console.WriteLine("A list of all room types available: ");
            foreach (var roomType in allRoomTypes)
            {
                Console.WriteLine(roomType);
            }
            PrintNotification("Be advised that this is a highly discouraged action.\nJust because you can, does not mean you should!");
            PrintNotification("Any changes here will only affect future bookings");
            PrintNotification("These are the available properties you can change: ");
            var choice = UserInputValidation.MenuValidation(_roomTypeProperties, "Choose an option: ");
            if (choice == -1) { return; }
            else
            {
                if (choice != _roomTypeProperties.Count)
                {
                    Console.WriteLine($"You chose to update {_roomTypeProperties[choice]}");
                    Console.Write("Enter the new value: ");
                }
                string? updatedValue;
                int? updatedBedValue;
                var roomType = allRoomTypes[choice - 1];
                switch (choice)
                {
                    case 1:
                        updatedValue = UserInputValidation.AskForValidName("suite name", 10, 30);
                        if (updatedValue == null) { return; }
                        roomType.SuiteName = updatedValue;
                        break;
                    case 2:
                        updatedBedValue = (int?)UserInputValidation.AskForValidNumber(0, 2, "test");
                        if (updatedBedValue == null) { return; }
                        roomType.NumberOfExtraBeds = (int)updatedBedValue;
                        break;
                    case 3:
                        Console.WriteLine($"Is double room: {roomType.IsDoubleRoom}. Be warned that changing this to false will set the number of extra beds to 0.");
                        roomType.IsDoubleRoom = !roomType.IsDoubleRoom;
                        if (roomType.IsDoubleRoom) { roomType.NumberOfExtraBeds = 0; }
                        break;
                }
                DbContext.SaveChanges();
            }
            PressAnyKeyToContinue();
        }
        private static void PrintAllAvailableRooms(HotelContext ctx)
        {
            var roomsByCategory = ctx.Room.Where(r => r.IsActive)
                .Include(rt => rt.RoomType)
                .GroupBy(rt => rt.RoomType)
                .Select(c => new
                {
                    SuiteName = c.Key,
                    NumberOfRooms = c.ToList(),
                    HeaderLength = c.Max(r => r.Description.Length),
                }).ToList();

            foreach (var category in roomsByCategory)
            {
                var header = new string('-', category.HeaderLength);
                PrintSuccessMessage($"All of our available {category.SuiteName.SuiteName}s");
                foreach (var room in category.NumberOfRooms)
                {
                    var info = RoomTemplate(room, header);
                    Console.WriteLine(info);
                }
                Console.WriteLine(header);
            }
        }
        public void SoftDelete(HotelContext ctx)
        {
            Console.Clear();
            PrintNotification("You can only delete rooms without any future bookings.");
            Console.Write("These are the current rooms without any future bookings: ");
            var roomsWithoutFutureBookings = ctx.Room.Where(b => b.Bookings.All(b => b.EndDate <= DateTime.Today));

            foreach (var room in roomsWithoutFutureBookings)
            {
                Console.WriteLine(room);
                Console.WriteLine("Would you like to (soft) delete it?");
                if (UserInputValidation.PromptYesOrNo("Press y to confirm, anything else to deny: "))
                {
                    room.IsActive = !room.IsActive;
                }
                PrintNotification($"Status has been changed to deleted");
                ctx.SaveChanges();
            }
            PressAnyKeyToContinue();
        }
        public void GeneralSearch(HotelContext ctx)
        {
            Console.Clear();
            Console.Write("Enter your room number: ");
            var roomNumber = UserInputValidation.AskForValidInputString();
            if (roomNumber == null)
            {
                PrintErrorMessage("No such room exists");
            }
            else
            {
                var allRooms = ctx.Room
                    .Where(r => r.RoomNumber.Contains(roomNumber))
                    .Include(rt => rt.RoomType)
                    .GroupBy(rt => rt.RoomType)
                    .Select(c => new
                    {
                        c.Key.SuiteName,
                        NumberOfRooms = c.ToList(),
                    });
                Console.Clear();
                Console.WriteLine($"These are all the rooms that contains the room number sequence \"{roomNumber}\"");
                foreach (var item in allRooms)
                {
                    PrintNotification($"Suite type - {item.SuiteName}");
                    foreach (var room in item.NumberOfRooms)
                    {
                        Console.WriteLine(room);
                    }
                    Console.WriteLine();
                }
            }
            PressAnyKeyToContinue();
        }
        public static Room? GetRoom(string roomNumber, HotelContext ctx)
        {
            var chosenRoom = ctx.Room
            .Where(r => r.RoomNumber
            .Equals(roomNumber))
            .Include(rt => rt.RoomType)
            .Include(b => b.Bookings)
            .FirstOrDefault();
            return chosenRoom;
        }
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
        public static void Sort(HotelContext ctx)
        {
            var query = ctx.Review
                .Include(rv => rv.Room)
                .GroupBy(rv => rv.Room.RoomNumber)
                .Select(c => new
                {
                    RoomNumber = c.Key,
                    SumOfStars = Math.Round(c.Average(rv => rv.StarsGiven) / 10, 1)
                }).OrderByDescending(r => r.SumOfStars);

            foreach (var result in query)
            {
                string info = $"|{result.RoomNumber}|{result.SumOfStars}|";
                Console.WriteLine(info);
                //Console.WriteLine($"RoomNumber: {result.RoomNumber}, SumOfStars: {result.SumOfStars}");
            }
            PressAnyKeyToContinue();
        }
    }
}
