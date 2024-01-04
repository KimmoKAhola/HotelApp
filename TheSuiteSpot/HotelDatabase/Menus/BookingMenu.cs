using Autofac.Features.AttributeFilters;
using TheSuiteSpot.HotelDatabase.CRUD;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class BookingMenu(HotelContext dBcontext, BookingCRUD booking) : IMenu
    {
        public HotelContext DbContext { get; set; } = dBcontext;
        public string MenuName { get; set; } = "Handle Bookings";
        public List<IMenu> Menus { get; set; }

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    booking.Create(DbContext);
                    break;
                case 2:
                    booking.CreateManually();
                    break;
                case 3:
                    booking.ReadAll(DbContext);
                    break;
                case 4:
                    booking.Update(DbContext);
                    break;
                case 5:
                    booking.SoftDelete(DbContext);
                    break;
                case 6:
                    booking.GeneralSearch(DbContext);
                    break;
                case 7:
                    ReturnToMainMenu();
                    break;
                case 0:
                    Environment.Exit(0);
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Create booking (auto-booking).");
            Console.WriteLine("2. Create a booking (manually).");
            Console.WriteLine("3. Read all bookings.");
            Console.WriteLine("4. Update a booking.");
            Console.WriteLine("5. Delete a booking.");
            Console.WriteLine("6. Search for a booking.");
            Console.WriteLine("7. Return to main menu.");
        }

        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }
    }
}