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
    public class UserMenu(HotelContext dbContext, UserCRUD user) : IMenu
    {
        public HotelContext DbContext { get; set; } = dbContext;
        public string MenuName { get; set; } = "Handle Users";

        public List<IMenu> Menus { get; set; }

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    user.Create(DbContext);
                    break;

                case 2:
                    user.ExactSearch(DbContext);
                    break;

                case 3:
                    user.ReadAll(DbContext);
                    break;

                case 4:
                    user.Update(DbContext);
                    break;

                case 5:
                    user.SoftDelete(DbContext);
                    break;

                case 6:
                    user.GeneralSearch(DbContext);
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;
            }

        }

        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Create a new user.");
            Console.WriteLine("2. Search for username.");
            Console.WriteLine("3. Read all users.");
            Console.WriteLine("4. Update a user.");
            Console.WriteLine("5. Delete a user.");
            Console.WriteLine("6. Search for a user by phrase.");
            Console.WriteLine("0. Return to main menu.");
            Console.Write("Choose an option: ");
        }
    }
}
