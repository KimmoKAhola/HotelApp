using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Services.CRUD;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class InvoiceMenu(HotelContext dbContext, InvoiceCRUD invoice) : IMenu
    {
        public HotelContext DbContext { get; set; } = dbContext;
        public string MenuName { get; set; } = "Handle Invoices";
        public List<IMenu> Menus { get; set; }

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    invoice.ReadAll(DbContext);
                    break;
                case 2:
                    invoice.Update(DbContext);
                    break;
                case 3:
                    invoice.SoftDelete(DbContext);
                    break;
                case 4:
                    invoice.GeneralSearch(DbContext);
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;

            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Read all unpaid invoices.");
            Console.WriteLine("2. Update an invoice.");
            Console.WriteLine("3. Delete an invoice.");
            Console.WriteLine("4. Search for an invoice by general phrase.");
            Console.WriteLine("0. Return to main menu.");

        }
        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }
    }
}