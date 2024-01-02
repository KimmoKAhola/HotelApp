using TheSuiteSpot.HotelDatabase.CRUD;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
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

                    break;
                case 2:
                    invoice.ExactSearch(DbContext);
                    break;
                case 3:
                    invoice.ReadAll(DbContext);
                    break;
                case 4:
                    invoice.Update(DbContext);
                    break;
                case 5:
                    invoice.SoftDelete(DbContext);
                    break;
                case 6:
                    invoice.GeneralSearch(DbContext);
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;

            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Create a new invoice. ------- Remove this");
            Console.WriteLine("2. Exact search for invoice.");
            Console.WriteLine("3. Read all invoices.");
            Console.WriteLine("4. Update an invoice.");
            Console.WriteLine("5. Delete an invoice.");
            Console.WriteLine("6. Search for an invoice by general phrase.");
            Console.WriteLine("0. Return to main menu.");

        }

        //Console.WriteLine("1. Create booking.");
        //    Console.WriteLine("2. Read a single booking.");
        //    Console.WriteLine("3. Read all bookings.");
        //    Console.WriteLine("4. Update a booking.");
        //    Console.WriteLine("5. Delete a booking.");
        //    Console.WriteLine("6. Search for a booking.");
        //    Console.WriteLine("7. Return to main menu.");
        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }
    }
}