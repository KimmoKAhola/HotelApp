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
            {3, "Room Price."},
            {4, "Room booking availability."},
        };
        public void Create(HotelContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Create a new hotel room. These are our available room types:\n");
            var roomTypes = ctx.RoomType.ToList();
            var input = UserInputValidation.MenuValidation(roomTypes, "Choose a suite type: ");
            PrintNotification($"You chose: {input}\n");
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
            PrintNotification($"You chose room number: {roomNumber}\n");
            var roomSize = (int?)UserInputValidation.AskForValidNumber(20, 200, "Enter a value (m²) for the room size");
            if (roomSize == null) { return; }
            var room = new Room
            {
                RoomType = roomTypes[input - 1],
                RoomNumber = roomNumber,
                RoomSize = (int)roomSize
            };
            var roomDescription = UserInputValidation.AskForValidInputString("description");
            if (roomDescription == null) { return; }
            if (roomDescription.Length > 500)
            {
                roomDescription = roomDescription.Substring(0, 499);
            }
            room.Description = roomDescription;
            Console.WriteLine();
            var pricePerDay = UserInputValidation.AskForValidNumber(1000, 20000, "Enter a price per day for the room in SEK");
            if (pricePerDay == null) { return; }
            if (room.RoomType.SuiteName.Contains("Deluxe") || room.RoomType.SuiteName.Contains("Royal"))
            {
                var pricePerExtraBed = UserInputValidation.AskForValidNumber(200, 3000, "Enter a price per day per each extra bed. ");
                room.PricePerExtraBed = pricePerExtraBed;
            }
            room.PricePerDay = (decimal)pricePerDay;
            Console.Clear();
            PrintSuccessMessage("Your room: ");
            var header = new string('-', 10);
            var info = RoomTemplate(room, header);
            Console.WriteLine(info);
            if (UserInputValidation.PromptYesOrNo("Press y to save this room, anything else to discard: "))
            {
                ctx.Add(room);
                ctx.SaveChanges();
                PrintSuccessMessage("Room has been added successfully.");
            }
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
                var header = new string('-', 40);
                if (searchResult.Description.Length > 40)
                {
                    header = new string('-', searchResult.Description.Length);
                }
                var info = RoomTemplate(searchResult, header);
                Console.WriteLine(info);
                Console.WriteLine(header);
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
                var header = new string('-', 40);

                if (room.Description.Length > 40)
                {
                    header = new string('-', room.Description.Length);
                }
                var info = RoomTemplate(room, header);
                Console.WriteLine(info);
            }
            Console.Write("Enter a room number to update that room: ");
            var roomNumber = UserInputValidation.AskForValidInputString();
            if (roomNumber == null) { return; }
            var chosenRoom = GetRoom(roomNumber, ctx);
            if (chosenRoom != null)
            {
                string? updatedValue;
                bool isRunning = true;
                while (isRunning)
                {
                    var choice = UserInputValidation.MenuValidation(_modelProperties, "\nChoose an option to update. ");
                    if (choice != _modelProperties.Count)
                    {
                        PrintNotification($"You chose to update {_modelProperties[choice]}\n");
                    }
                    switch (choice)
                    {
                        case 1:
                            updatedValue = UserInputValidation.AskForValidInputString("description");
                            if (updatedValue == null) { return; }
                            chosenRoom.Description = updatedValue;
                            break;
                        case 2:
                            updatedValue = UserInputValidation.AskForValidRoomNumber("room number", 4, 6);
                            if (updatedValue == null) { return; }
                            if (ctx.Room.Any(r => r.RoomNumber == updatedValue))
                            {
                                PrintErrorMessage("That room number is already taken. Changes has not been applied.");
                            }
                            else
                            {
                                chosenRoom.RoomNumber = updatedValue;
                            }
                            break;
                        case 3:
                            decimal? value = UserInputValidation.AskForValidNumber(1000m, 10000m, "Update the price per day.");
                            if (value == null) { return; }
                            PrintNotification($"You chose {value:C2}");
                            chosenRoom.PricePerDay = (decimal)value;
                            break;
                        case 4:
                            Console.Clear();
                            PrintNotification($"The current booking availability is: {chosenRoom.IsActive}");
                            PrintErrorMessage("Please be warned that you are about to commit a destructive action!");
                            if (UserInputValidation.PromptYesOrNo("You wish to change a room's availability. Please note that if this is changed to false, it will not be available for any new bookings." +
                                "\nPress y to confirm, anything else to discard: "))
                            {
                                chosenRoom.IsActive = !chosenRoom.IsActive;
                                PrintNotification($"Availability has been changed to: {chosenRoom.IsActive}\n");
                            }
                            break;
                    }
                    if (!UserInputValidation.PromptYesOrNo("Change another property? (y/n): \n"))
                    {
                        isRunning = false;
                        break;
                    }
                }
                Console.Clear();
                PrintSuccessMessage("Your room has been changed to: ");
                var newHeader = new string('-', chosenRoom.Description.Length);
                var newInfo = RoomTemplate(chosenRoom, newHeader);
                Console.WriteLine(newInfo);
                if (UserInputValidation.PromptYesOrNo("Press y to confirm changes, anything else to discard: "))
                {
                    PrintSuccessMessage("Your changes has been applied.");
                    ctx.SaveChanges();
                }
                else
                {
                    PrintNotification("Changes has been discarded");
                }
            }
            else
            {
                PrintErrorMessage($"No room with the room number \"{roomNumber}\" exists");
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
            var roomsWithoutFutureBookings = ctx.Room
                .Where(r => r.IsActive)
                .Include(rt => rt.RoomType)
                .Include(b => b.Bookings)
                .Where(b => b.Bookings.All(b => b.EndDate <= DateTime.Today) || !b.Bookings.Any());
            if (roomsWithoutFutureBookings.Any())
            {
                PrintNotification($"Your result yielded {roomsWithoutFutureBookings.Count()} results. One result will be shown at a time");
                Console.Write("These are the current rooms without any future bookings: \n");
                foreach (var room in roomsWithoutFutureBookings)
                {
                    var header = new string('-', 40);
                    var info = RoomTemplate(room, header);
                    Console.WriteLine(info);
                    Console.WriteLine(header);
                    Console.WriteLine("Would you like to (soft) delete it?");
                    if (UserInputValidation.PromptYesOrNo("Press y to confirm, anything else to deny:"))
                    {
                        PrintNotification($"Status has been changed to deleted\n");
                        room.IsActive = !room.IsActive;
                    }
                    else
                    {
                        PrintNotification("Room has not been deleted.\n");
                    }
                    ctx.SaveChanges();
                }
            }
            else
            {
                PrintNotification("No rooms can be deleted.");
            }
            PressAnyKeyToContinue();
        }
        public void GeneralSearch(HotelContext ctx)
        {
            Console.Clear();
            PrintNotification("You can search for a general search term to find all rooms containing that term. Try to be specific.\n");
            Console.Write("Enter a room number sequence to search for: ");
            var roomNumber = UserInputValidation.AskForValidInputString();
            if (roomNumber == null)
            {
                PrintErrorMessage("No such room exists");
            }
            else
            {
                Console.Clear();
                var allRooms = ctx.Room
                    .Where(r => r.RoomNumber.Contains(roomNumber))
                    .Include(rt => rt.RoomType)
                    .GroupBy(rt => rt.RoomType)
                    .Select(c => new
                    {
                        c.Key.SuiteName,
                        NumberOfRooms = c.ToList(),
                    });
                Console.WriteLine($"These are all the rooms that contains the room number sequence \"{roomNumber}\"");
                foreach (var item in allRooms)
                {
                    PrintNotification($"Suite type - {item.SuiteName}");
                    foreach (var room in item.NumberOfRooms)
                    {
                        var header = new string('-', 40);
                        if (room.Description.Length > 40)
                        {
                            header = new string('-', room.Description.Length);
                        }
                        var info = RoomTemplate(room, header);
                        Console.WriteLine(info);
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
    }
}
