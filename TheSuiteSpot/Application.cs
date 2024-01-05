using Autofac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.DatabaseSeeding;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.HotelDatabase.Models;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;
using TheSuiteSpot.Interfaces;
using TheSuiteSpot.HotelDatabase.Services;
using TheSuiteSpot.HotelDatabase.Menus.UserMenus;

namespace TheSuiteSpot
{
    public class Application(HotelContext dbContext) : IApplication
    {
        public HotelContext DbContext { get; set; } = dbContext;

        private void CheckInvoices()
        {
            var invoices = DbContext.Invoice
    .Include(i => i.Booking)
        .ThenInclude(b => b.User)
            .ThenInclude(u => u.UserInbox)
    .Where(i => i.DueDate < DateTime.Today && !i.IsPaid && i.Booking.User.UserName != "Richard")
    .ToList();

            foreach (var invoice in invoices)
            {
                invoice.IsActive = false;
                invoice.Booking.IsActive = false;
                string topic = "Booking canceled";
                string content = "Dear sir/mam, since we have not yet received a payment for your booking your booking has been canceled.";
                SystemMessageServices.SendSystemMessage(DbContext, invoice.Booking.User, topic, content);
            }
            DbContext.SaveChanges();
        }

        private void CheckVouchers()
        {
            var allVouchers = DbContext.Voucher
                .Where(v => v.ExpiryDate < DateTime.Today && !v.IsExpired);

            foreach (var voucher in allVouchers)
            {
                voucher.IsExpired = true;
            }
            DbContext.SaveChanges();
        }
        public void Run(IContainer container)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var initialize = scope.Resolve<Initialize>();
                initialize.Seed();
                CheckInvoices();
                CheckVouchers();
                Login();
                if (CurrentUser.Instance.User.IsAdmin)
                {
                    var mainMenu = scope.Resolve<MainMenu>();

                    while (true)
                    {
                        try
                        {
                            mainMenu.Display();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.InnerException);
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();
                        }
                    }
                }
                else
                {
                    var mainMenu = scope.Resolve<UserMainMenu>();
                    while (true)
                    {
                        try
                        {
                            mainMenu.Display();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.InnerException);
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }

        public void Login()
        {
            PrintNotification("Welcome to the login page. Please enter username and password.\n(leave blank to login as admin, use username = Richard and password = 123456 to log in as Richard.");
            var allUsers = DbContext.User;
            Console.Write("Enter a username: ");
            var username = Console.ReadLine();
            Console.Write("Enter a password: ");
            var password = Console.ReadLine();
            if (GetAuthority(username, password, allUsers))
            {
                PrintNotification("Login successful!");
                PressAnyKeyToContinue();
            }
        }

        private bool GetAuthority(string username, string password, DbSet<User> user)
        {
            if (user.Where(u => u.UserName == username).Any() && user.Where(u => u.Password == password).Any())
            {
                var currentuser = user.Where(u => u.UserName == username && u.Password == password).First();
                CurrentUser.SetCurrentUser(currentuser);
                return true;
            }
            else
            {
                CurrentUser.SetCurrentUser(DbContext.User.Where(u => u.IsAdmin && u.UserName == "admin").First());
                return false;
            }
        }
    }
}
