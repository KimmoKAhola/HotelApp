using Microsoft.EntityFrameworkCore;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using static TheSuiteSpot.HotelDatabase.InputHelpers.PrintMessages;

namespace TheSuiteSpot.HotelDatabase.Menus
{
    public class InboxMenu(HotelContext dbContext) : IMenu
    {
        public string MenuName { get; set; } = "Inbox";
        public HotelContext DbContext { get; set; } = dbContext;
        public List<IMenu> Menus { get; set; }

        public void Display()
        {
            MainMenu.PrintBanner();
            PrintOptions();
            var input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    ShowUnreadMessages();
                    break;
                case 2:
                    ShowSentMessages(CurrentUser.Instance.User);
                    break;
                case 3:
                    ShowReceivedMessages(CurrentUser.Instance.User);
                    break;
                case 4:
                    SendMessageToUser();
                    break;
                case 0:
                    ReturnToMainMenu();
                    break;
            }
        }

        public void SendMessageToUser()
        {
            throw new NotImplementedException();
        }

        public void PrintOptions()
        {
            //var unreadMessages = DbContext.User
            //    .Where(u => u.Id == CurrentUser.Instance.User.Id)
            //    .Include(u => u.UserInbox
            //    .Where(m => !m.IsRead)).First();
            //var numberOfUnreadMessages = unreadMessages.UserMessages.Count;
            MainMenu.PrintBanner();
            Console.WriteLine($"You currently have unread messages."); //{numberOfUnreadMessages}
            Console.WriteLine("1. View unread messages.");
            Console.WriteLine("2. View all sent system messages.");
            Console.WriteLine("3. View all received messages.");
            Console.WriteLine("4. Send a message to a user.");
            Console.WriteLine("0. Return to main menu.");
        }

        public void ReturnToMainMenu()
        {
            _ = new MainMenu(Menus);
        }

        public string? FilterByTopic()
        {
            Console.Write("Enter a topic (invoice, booking, user) to filter (leave blank to read all sent messages): ");
            var input = Console.ReadLine();
            return input;
        }

        public void ShowSentMessages(User loggedInUser)
        {
            PrintErrorMessage("FIX MY FORMATTING PLEASE!!!");
            var filter = FilterByTopic();
            if (filter != null)
            {
                var allSentMessages = DbContext.User
                    .Include(u => u.UserInbox)
                    .ThenInclude(m => m.Messages
                    .Where(m => m.Sender == loggedInUser.UserName))
                    .Select(c => new
                    {
                        messages = c.UserInbox.Messages.Where(m => m.Sender == loggedInUser.UserName).ToList()
                    }).Where(c => c.messages.Count > 0).ToList();
                foreach (var item in allSentMessages)
                {
                    foreach (var message in item.messages)
                    {
                        Console.WriteLine(message.Content);
                    }
                }
            }
            else
            {
                var allSentMessages = DbContext.User
                    .Include(u => u.UserInbox)
                    .ThenInclude(m => m.Messages
                    .Where(m => m.Sender == loggedInUser.UserName && m.Topic.ToLower() == filter.ToLower()))
                    .Select(c => new
                    {
                        messages = c.UserInbox.Messages.Where(m => m.Sender == loggedInUser.UserName).ToList()
                    }).Where(c => c.messages.Count > 0).ToList();
                foreach (var item in allSentMessages)
                {
                    foreach (var message in item.messages)
                    {
                        Console.WriteLine(message.Content);
                    }
                }
            }

            PressAnyKeyToContinue();
        }
        public void ShowUnreadMessages()
        {
            //Console.Clear();
            //var unreadMessages = DbContext.User
            //    .Where(u => u.Id == CurrentUser.Instance.User.Id)
            //    .Include(u => u.UserInbox
            //    .Where(m => !m.IsRead)).First();
            //foreach (var unreadMessage in unreadMessages.UserMessages)
            //{
            //    Console.WriteLine($"Sent by: {unreadMessage.Sender}");
            //    Console.WriteLine($"time stamp: {unreadMessage.MessageTimeStamp}");
            //    var receiver = DbContext.User.FirstOrDefault(u => u.Id == unreadMessage.Receiver.Id);
            //    Console.WriteLine($"Message text: {unreadMessage.MessageText}");
            //    Console.WriteLine($"Sent to: {receiver.UserName}");
            //    Console.WriteLine();
            //    unreadMessage.IsRead = true;
            //    DbContext.SaveChanges();
            //    Console.WriteLine("Press any key to continue to the next unread message.");
            //    Console.ReadKey();
            //    Console.Clear();
            //}
            //Console.WriteLine("Press any key to continue.");
            //Console.ReadKey();
        }

        public void ShowReceivedMessages(User loggedInUser)
        {
            var allReceivedMessages = DbContext.User
                    .Where(u => u.Id == loggedInUser.Id)
                    .Include(u => u.UserInbox)
                    .ThenInclude(m => m.Messages)
                    .Select(c => new
                    {
                        msg = c.UserInbox.Messages.ToList(),
                    });

            foreach (var item in allReceivedMessages)
            {
                foreach (var msg in item.msg)
                {
                    Console.WriteLine(msg.Content);
                }
                PressAnyKeyToContinue();
            }
        }
    }
}