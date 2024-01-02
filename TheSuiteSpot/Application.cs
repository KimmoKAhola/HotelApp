using Autofac;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.DatabaseSeeding;
using TheSuiteSpot.HotelDatabase.InputHelpers;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.HotelDatabase.UserMenus;

namespace TheSuiteSpot
{
    public class Application(HotelContext dbContext) : IApplication
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void CheckInvoices()
        {
            //var invoices = DbContext.Invoice.ToList();
            //Console.WriteLine("I AM CHECKING THE INVOICES");
            //Console.ReadKey();
        }
        public void Run(IContainer container)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var initialize = scope.Resolve<Initialize>();
                initialize.Seed();
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

        private void Login()
        {
            var allUsers = DbContext.User;
            Console.Write("Enter a username: ");
            var username = Console.ReadLine();
            Console.Write("Enter a password: ");
            var password = Console.ReadLine();
            if (GetAuthority(username, password, allUsers))
            {
                PrintMessages.PrintNotification("Login successful!");
                PrintMessages.PressAnyKeyToContinue();
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
                CurrentUser.SetCurrentUser(DbContext.User.Where(u => u.IsAdmin).First());
                return false;
            }
        }
    }
}
