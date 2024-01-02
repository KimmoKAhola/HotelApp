using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.CRUD;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class AdminMenu(HotelContext dbContext, AdminCRUD crud) : IMenu
    {
        public string MenuName { get; set; } = "Admin tools";
        public List<IMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public AdminCRUD Crud { get; set; } = crud;

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Crud.Create(DbContext);
                    break;
                case 2:
                    Crud.CreateSystemMessage(DbContext);
                    break;
                case 3:
                    Crud.ViewMostPopularRooms(DbContext);
                    break;
                case 4:
                    Crud.ViewHighestSpenders(DbContext);
                    break;
            }
            PrintMessages.PressAnyKeyToContinue();
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Send vouchers to big spenders.");
            Console.WriteLine("2. Send a system-wide message to all users.");
            Console.WriteLine("3. View our most popular rooms.");
            Console.WriteLine("4. View our highest spenders");
            Console.WriteLine("1. ");
            Console.WriteLine("1. ");
            Console.WriteLine("1. ");
            Console.WriteLine("1. ");
        }

        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }
    }
}



//var userAverageSpending = ctx.Invoice
//               .Where(i => !i.IsPaid)
//               .GroupBy(i => i.Booking.User.Id)
//               .Select(c => new
//               {
//                   UserId = c.Key,
//                   AverageSpending = c.Average(i => i.Amount)
//               }).Average(u => u.AverageSpending);

//var bigSpenders = ctx.Invoice
//                .Where(i => !i.IsPaid || i.IsPaid)
//                .Include(b => b.Booking)
//                .ThenInclude(u => u.User)
//                .GroupBy(u => u.Booking.User)
//                .Select(c => new
//                {
//                    User = c.Key,
//                    AverageSpending = c.Average(i => i.Amount)
//                })
//                .Where(user => user.AverageSpending >=
//                    ctx.Invoice
//                        .Where(i => !i.IsPaid)
//                        .GroupBy(i => i.Booking.User)
//                        .Select(c => c.Average(i => i.Amount))
//                        .Average())
//                .ToList();