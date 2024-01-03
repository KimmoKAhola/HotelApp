using Microsoft.EntityFrameworkCore;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;

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
            var username = UserInputValidation.AskForValidInputString("username");
            if (username == null) { return; }
            if (DbContext.User.Any(u => u.UserName == username))
            {
                var receiver = DbContext.User
                    .Where(u => u.UserName == username)
                    .Include(u => u.UserInbox).First();

                var topic = UserInputValidation.AskForValidInputString("message topic");
                if (topic == null) { return; }
                var content = UserInputValidation.AskForValidInputString("message content");
                if (content == null) { return; }

                SystemMessage.SendMessageBetweenUsers(DbContext, CurrentUser.Instance.User, receiver, topic, content);
                PrintSuccessMessage("Message has been sent.");
            }
            PressAnyKeyToContinue();
        }

        public void PrintOptions()
        {
            var unreadMessages = DbContext.User
                .Where(u => u.UserName == CurrentUser.Instance.User.UserName)
                .Include(u => u.UserInbox)
                .ThenInclude(m => m.Messages.Where(x => !x.IsRead));

            MainMenu.PrintBanner();
            Console.WriteLine($"You currently have {unreadMessages.Count()} unread messages."); //{numberOfUnreadMessages}
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
                        message.IsRead = true;
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
                        message.IsRead = true;
                    }
                }
            }

            PressAnyKeyToContinue();
        }
        public void ShowUnreadMessages()
        {
            var unreadMessages = DbContext.User
                .Where(u => u.UserName == CurrentUser.Instance.User.UserName)
                .Include(u => u.UserInbox)
                .ThenInclude(m => m.Messages.Where(m => !m.IsRead))
                .Select(c => new
                {
                    msg = c.UserInbox.Messages.ToList(),
                });

            foreach (var message in unreadMessages)
            {
                foreach (var msg in message.msg)
                {
                    Console.WriteLine(msg.Content);
                    PressAnyKeyToContinue();
                }
            }
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
                    msg.IsRead = true;
                }
                PressAnyKeyToContinue();
            }
        }
    }
}