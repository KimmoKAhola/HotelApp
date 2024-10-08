﻿using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Services.CRUD;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Menus.UserMenus
{
    public class UserBookingMenu(HotelContext dbContext, BookingCRUD booking) : IUserMenu
    {
        public string MenuName { get; set; } = "Bookings";
        public List<IUserMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

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
                    booking.Update(DbContext);
                    break;
                case 3:
                    booking.ReadAll(DbContext);
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. Create a new booking.");
            Console.WriteLine("2. Update a booking.");
            Console.WriteLine("3. View your bookings.");
            Console.WriteLine("0. Return to the main menu.");
        }

        public void ReturnToMainMenu()
        {
            _ = new UserMainMenu(Menus);
        }
    }
}
