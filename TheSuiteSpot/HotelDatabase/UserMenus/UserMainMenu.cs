using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.UserMenus
{
    public class UserMainMenu(List<IUserMenu> menus)
    {
        private readonly List<IUserMenu> _menus = menus;
        public void Display()
        {
            MainMenu.PrintBanner();
            for (int menuIndex = 0; menuIndex < _menus.Count; menuIndex++)
            {
                Console.WriteLine($"{menuIndex + 1}. {_menus[menuIndex].MenuName}");
            }
            Console.WriteLine($"{_menus.Count + 1}. Exit the application.");

            Console.Write("Choose an option: ");
            var chosenIndex = Convert.ToInt32(Console.ReadLine()) - 1;

            if (chosenIndex < _menus.Count)
            {
                _menus.ElementAt(chosenIndex).Display();
            }
            else if (chosenIndex == _menus.Count)
            {
                Environment.Exit(0);
            }
        }
    }
}
