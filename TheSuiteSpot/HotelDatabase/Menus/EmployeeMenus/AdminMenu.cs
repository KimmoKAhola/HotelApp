using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using TheSuiteSpot.HotelDatabase.Services.CRUD;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class AdminMenu(HotelContext dbContext, AdminCRUD crud) : IMenu
    {
        public string MenuName { get; set; } = "Admin tools";
        public List<IMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public AdminCRUD Crud { get; set; } = crud;

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Crud.Create(DbContext);
                    break;
                case 2:
                    Crud.CreateSystemMessage(DbContext);
                    break;
                case 3:
                    Crud.ViewMostPopularRooms(DbContext);
                    break;
                case 4:
                    Crud.ViewHighestSpenders(DbContext);
                    break;
            }
            PrintMessages.PressAnyKeyToContinue();
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Send vouchers to big spenders.");
            Console.WriteLine("2. Send a system-wide message to all users.");
            Console.WriteLine("3. View our most popular rooms.");
            Console.WriteLine("4. View our highest spenders");
            Console.Write("Choose an option: ");
        }

        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }
    }
}