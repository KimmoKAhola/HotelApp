using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Services.CRUD;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class RoomMenu(HotelContext dbContext, RoomCRUD room) : IMenu
    {
        public HotelContext DbContext { get; set; } = dbContext;
        public string MenuName { get; set; } = "Handle Rooms";
        public List<IMenu> Menus { get; set; }

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    room.Create(DbContext);
                    break;

                case 2:
                    room.ExactSearch(DbContext);
                    break;

                case 3:
                    room.ReadAll(DbContext);
                    break;

                case 4:
                    room.Update(DbContext);
                    break;
                case 5:
                    room.SoftDelete(DbContext);
                    break;
                case 6:
                    room.GeneralSearch(DbContext);
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Create a new room.");
            Console.WriteLine("2. Read a single rooms.");
            Console.WriteLine("3. Read all rooms.");
            Console.WriteLine("4. Update a room.");
            Console.WriteLine("5. Delete a room.");
            Console.WriteLine("6. Search for a room.");
            Console.WriteLine("0. Return to main menu.");
            Console.Write("Choose: ");
        }

        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }
    }
}
