using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.UserMenus
{
    public class UserInboxMenu(HotelContext dbContext, InboxMenu inboxMenu) : IUserMenu
    {
        public string MenuName { get; set; } = "View inbox";
        public List<IUserMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public InboxMenu InboxMenu { get; set; } = inboxMenu;

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var choice = Convert.ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    InboxMenu.ShowUnreadMessages();
                    break;
                case 2:
                    InboxMenu.ShowSentMessages(CurrentUser.Instance.User);
                    break;
                case 3:
                    InboxMenu.ShowReceivedMessages(CurrentUser.Instance.User);
                    break;
                case 4:
                    InboxMenu.SendMessageToUser();
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void PrintOptions()
        {
            Console.WriteLine("1. View unread messages.");
            Console.WriteLine("2. View sent messages.");
            Console.WriteLine("3. View received messages.");
            Console.WriteLine("4. Send message to another user.");
        }

        public void ReturnToMainMenu()
        {
            _ = new UserMainMenu(Menus);
        }
    }
}
