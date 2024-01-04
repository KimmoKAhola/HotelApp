using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.CRUD;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Menus.UserMenus
{
    public class UserSettingsMenu(HotelContext dbContext, UserCRUD crud) : IUserMenu
    {
        public string MenuName { get; set; } = "Settings menu";
        public List<IUserMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public UserCRUD Crud { get; set; } = crud;

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();

            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Crud.Update(DbContext);
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("Implement ways to change user info here.");
            Console.WriteLine("1. Change user settings.");
            Console.WriteLine("0. Return to main menu.");
        }

        public void ReturnToMainMenu()
        {
            _ = new UserMainMenu(Menus);
        }
    }
}
