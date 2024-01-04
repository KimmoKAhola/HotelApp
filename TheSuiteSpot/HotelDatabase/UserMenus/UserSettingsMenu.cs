using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.UserMenus
{
    public class UserSettingsMenu(HotelContext dbContext) : IUserMenu
    {
        public string MenuName { get; set; } = "Settings menu";
        public List<IUserMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();

            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("Implement ways to change user info here.");
        }

        public void ReturnToMainMenu()
        {
            _ = new UserMainMenu(Menus);
        }
    }
}
