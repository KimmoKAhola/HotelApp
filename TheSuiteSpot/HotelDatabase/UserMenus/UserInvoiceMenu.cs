using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.CRUD;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
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
            invoice.ReadAll(DbContext);
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. View your invoices.");
            Console.WriteLine("2. Pay an invoice.");
        }

        public void ReturnToMainMenu()
        {
            _ = new UserMainMenu(Menus);
        }
    }
}
