using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using TheSuiteSpot.HotelDatabase.Services;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class MainMenu(List<IMenu> menus)
    {
        private readonly List<IMenu> _menus = menus;
        public void Display()
        {
            PrintBanner();
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

        public static void PrintBanner()
        {
            Console.Clear();
            string banner = @"********************************************************************************************
*   #######                   #####                            #####                       *
*      #    #    # ######    #     # #    # # ##### ######    #     # #####   ####  #####  *
*      #    #    # #         #       #    # #   #   #         #       #    # #    #   #    *
*      #    ###### #####      #####  #    # #   #   #####      #####  #    # #    #   #    *
*      #    #    # #               # #    # #   #   #               # #####  #    #   #    *
*      #    #    # #         #     # #    # #   #   #         #     # #      #    #   #    *
*      #    #    # ######     #####   ####  #   #   ######     #####  #       ####    #    *
********************************************************************************************";

            Console.WriteLine(banner);
            PrintMessages.PrintNotification($"Currently logged in as user with username: {CurrentUser.Instance.User.UserName}");
        }
    }
}
