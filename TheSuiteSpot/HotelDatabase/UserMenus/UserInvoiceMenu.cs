using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.CRUD;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.UserMenus
{
    public class UserInvoiceMenu(HotelContext dbContext, InvoiceCRUD invoice) : IUserMenu
    {
        public string MenuName { get; set; } = "Handle invoices";
        public List<IUserMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    invoice.ReadAll(DbContext);
                    break;

                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. View your unpaid invoices.");
            Console.WriteLine("0. Return to the main menu.");
        }

        public void ReturnToMainMenu()
        {
            _ = new UserMainMenu(Menus);
        }
    }
}
