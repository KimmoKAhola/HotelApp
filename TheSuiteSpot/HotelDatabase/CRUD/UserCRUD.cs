using Azure.Identity;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Menus;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;
using System.Reflection.Metadata;
using System.ComponentModel.DataAnnotations;

namespace TheSuiteSpot.HotelDatabase.CRUD
{
    public class UserCRUD(HotelContext dbContext) : ICRUD
    {
        public HotelContext DbContext { get; set; } = dbContext;
        private readonly int _maxSearchResult = 10;
        private Dictionary<int, string> _modelProperties = new Dictionary<int, string>()
        {
            {1, "First name."},
            {2, "Last name."},
            {3, "Email."},
            {4, "Password."},
            {5, $"Subscription status."},
        };
        public void Create(HotelContext ctx)
        {
            Console.Clear();
            string? username;
            string? email;
            var firstName = UserInputValidation.AskForValidName("first name", 2, 30);
            if (firstName == null) { return; }
            var lastName = UserInputValidation.AskForValidName("last name", 2, 30);
            if (lastName == null) { return; }
            while (true)
            {
                username = UserInputValidation.AskForValidUsername("username", 6, 30);
                if (!ctx.User.Any(u => u.UserName == username))
                {
                    break;
                }
                PrintErrorMessage("That username is already taken.");
            }
            if (username == null) { return; }
            var password = UserInputValidation.AskForValidPassword("password", 6, 30);
            if (password == null) { return; }
            while (true)
            {
                email = UserInputValidation.AskForValidEmail("email", 6, 100);
                if (!ctx.User.Any(u => u.Email == email))
                {
                    break;
                }
                PrintErrorMessage("That email is already taken.");
            }
            if (email == null) { return; }
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = username,
                Password = password,
                Email = email,
                UserRole = ctx.UserRole.Where(r => r.RoleName.Contains("Guest")).First(),
                UserInbox = new UserInbox { },
            };
            string header = new string('-', user.Email.Length + 7);
            var info = FormatUserTable(user, header);
            PrintNotification("You have created this user: ");
            Console.WriteLine(info);
            Console.WriteLine(header);
            if (UserInputValidation.PromptYesOrNo("Press y to add this user to the system: "))
            {
                ctx.User.Add(user);
                ctx.SaveChanges();
                SystemMessage.SendCreatedUserMessage(user, ctx);
                Console.Clear();
                PrintSuccessMessage($"The user has been added to the system.");
            }
            PressAnyKeyToContinue();
        }

        public void SoftDelete(HotelContext ctx)
        {
            Console.Clear();
            PrintErrorMessage("Warning! If you delete a customer it can not be reverted!");
            Console.Write("Enter the username of the user you want to delete (admin can not be deleted): ");

            var input = UserInputValidation.AskForValidInputString();
            if (input == null) { return; }

            var userToDelete = Search(input);
            if (userToDelete != null && !userToDelete.IsAdmin)
            {
                if (BookingCRUD.CheckIfUserHasBookings(userToDelete, ctx))
                {
                    PrintErrorMessage("The user has active bookings and can not be deleted.");
                }
                else
                {
                    PrintSuccessMessage("Your search result yielded this user: ");
                    string header = new string('-', userToDelete.Email.Length + 7);
                    var info = FormatUserTable(userToDelete, header);
                    Console.WriteLine(info);
                    Console.WriteLine(header);
                    if (UserInputValidation.PromptYesOrNo("Press y to delete this user, any other key to decline: "))
                    {
                        userToDelete.IsActive = false;
                        PrintSuccessMessage($"The user with username {userToDelete.UserName} has been deleted.");
                        DbContext.SaveChanges();
                    }
                }
            }
            else
            {
                PrintErrorMessage("No such user exists in the database");
            }
            PressAnyKeyToContinue();
        }

        public void ReadAll(HotelContext ctx)
        {
            Console.Clear();
            PrintSuccessMessage("All active users: ");
            var users = ctx.User
                .Include(ur => ur.UserRole)
                .Where(u => u.IsActive && u.UserRole.RoleName != UserRoles.System.ToString())
                .OrderBy(u => u.Id)
                .ToList();

            var maxStringLength = new string('-', users.Max(c => c.Email.Length) + 7);

            foreach (var user in users)
            {
                string info = FormatUserTable(user, maxStringLength);
                Console.Write(info);
                if (user == users.Last())
                {
                    Console.WriteLine("\n" + maxStringLength);
                }
            }
            PressAnyKeyToContinue();
        }

        public static void ReadAllUserNames(HotelContext ctx)
        {
            Console.Clear();
            Console.WriteLine("All active users: ");
            var users = ctx.User
                .Where(u => u.IsActive && !u.IsAdmin)
                .OrderBy(u => u.Id);

            var maxLength = users.Max(c => c.UserName.Length);
            var divider = new string('-', maxLength);
            foreach (var item in users)
            {
                Console.WriteLine(divider);
                Console.WriteLine(item.UserName);
            }
        }

        public void ExactSearch(HotelContext ctx)
        {
            Console.Clear();
            Console.Write("Enter the exact username you want to search for: ");
            var input = UserInputValidation.AskForValidInputString();
            if (input == null) { return; }
            var user = ExactSearch(input);
            if (user != null)
            {
                string header = new string('-', user.Email.Length + 7);
                var info = FormatUserTable(user, header);
                PrintSuccessMessage("Your search yielded this user: ");
                Console.WriteLine(info);
                Console.WriteLine(header);
            }
            else
            {
                PrintErrorMessage("No user with that phrase exists.");
            }
            PressAnyKeyToContinue();
        }

        public User? GetUser()
        {
            PrintNotification("\nEnter the exact username you want to fetch: ");
            var input = Console.ReadLine();
            var user = ExactSearch(input);
            Console.Clear();
            if (user != null && !user.IsAdmin)
            {
                var header = new string('-', user.UserName.Length);
                PrintSuccessMessage("Your search result: ");
                var info = FormatUserTable(user, header);
                Console.WriteLine(info);
                Console.WriteLine(header);
            }
            else
            {
                PrintErrorMessage("No user with that username exists.");
                return null;
            }
            return user;
        }

        public void GeneralSearch(HotelContext ctx)
        {
            Console.Clear();
            var allUsers = DbContext.User.Where(u => u.IsActive);
            Console.WriteLine("This search returns all active users containing the chosen phrase.");
            Console.Write("Search by entering a username, first name, last name or email: ");
            var searchInput = UserInputValidation.AskForValidInputString();
            if (searchInput == null) { return; }
            var searchQuery = allUsers
                .Where(u => u.UserName.Contains(searchInput)
                || u.Email.Contains(searchInput)
                || u.FirstName.Contains(searchInput)
                || u.LastName.Contains(searchInput))
                .OrderBy(u => u.Id);
            Console.Clear();
            if (searchQuery.Any())
            {
                List<User> searchResult;
                if (searchQuery.Count() > _maxSearchResult)
                {
                    PrintNotification($"Your search result yielded more than {_maxSearchResult} users. Only the first {_maxSearchResult} users will be shown.");
                    searchResult = searchQuery.Take(_maxSearchResult).ToList();
                }
                else
                {
                    searchResult = searchQuery.ToList();
                }
                PrintSuccessMessage($"Result of your search: ");
                string header = new string('-', searchQuery.Max(u => u.Email.Length) + 7);
                foreach (var user in searchResult)
                {
                    var info = FormatUserTable(user, header);
                    Console.Write(info);
                    if (user == searchResult.Last())
                    {
                        Console.WriteLine("\n" + header);
                    }
                }
            }
            else
            {
                PrintErrorMessage($"Your search of {searchInput} did not produce any results.");
            }
            PressAnyKeyToContinue();
        }

        private User? Search(string userName)
        {
            var allUsers = DbContext.User.Where(u => u.IsActive);

            var searchResult = allUsers
                .Where(u => u.UserName.Contains(userName)
                || u.Email.Contains(userName)
                || u.FirstName.Contains(userName)
                || u.LastName.Contains(userName)
                && !u.IsAdmin).FirstOrDefault();

            return searchResult;
        }

        private User? ExactSearch(string inputString)
        {
            var allUsers = DbContext.User.Where(u => u.IsActive);
            var searchResult = allUsers
                .Where(u => u.UserName.ToLower()
                .Contains(inputString.ToLower()))
                .Include(ur => ur.UserRole)
                .Include(b => b.Bookings)
                .ThenInclude(r => r.Room)
                .ThenInclude(rt => rt.RoomType)
                .ToList();

            if (searchResult.Count > 1)
            {
                PrintNotification($"Your search of \"{inputString}\" yielded more than one result.\n" +
                    $"Only the first result is show here. If this was the incorrect user, please refine your search string.");
            }
            return searchResult.FirstOrDefault();
        }

        public void Update(HotelContext ctx)
        {
            Console.Clear();
            Console.Write("Enter a username you want to search for (case insensitive search): ");
            var input = UserInputValidation.AskForValidInputString();
            if (input == null) { return; }
            var user = ExactSearch(input);
            if (user != null && !user.IsAdmin)
            {
                PrintNotification("Your result yielded this user:");
                var header = new string('-', user.Email.Length + 7);
                var info = FormatUserTable(user, header);
                Console.Write(info);
                Console.WriteLine("\n" + header + "\n");
                string? updatedValue;
                bool isRunning = true;
                while (isRunning)
                {
                    var choice = UserInputValidation.MenuValidation(_modelProperties, "\nChoose an option to update. ");
                    if (choice != _modelProperties.Count)
                    {
                        PrintNotification($"You chose to update {_modelProperties[choice]}\n");
                        Console.Write("Enter the new value: ");
                    }
                    switch (choice)
                    {
                        case 1:
                            updatedValue = UserInputValidation.AskForValidName("first name", 2, 30);
                            if (updatedValue == null) { return; }
                            user.FirstName = updatedValue;
                            break;
                        case 2:
                            updatedValue = UserInputValidation.AskForValidName("last name", 2, 30);
                            if (updatedValue == null) { return; }
                            user.LastName = updatedValue;
                            break;
                        case 3:
                            updatedValue = UserInputValidation.AskForValidEmail("email name", 6, 100);
                            if (updatedValue == null) { return; }
                            user.Email = updatedValue;
                            break;
                        case 4:
                            updatedValue = UserInputValidation.AskForValidPassword("password", 6, 30);
                            if (updatedValue == null) { return; }
                            user.Password = updatedValue;
                            break;
                        case 5:
                            user.IsSubscriber = !user.IsSubscriber;
                            PrintNotification($"Subscription status has been changed to {user.IsSubscriber}");
                            break;
                    }
                    if (!UserInputValidation.PromptYesOrNo("Change another property? (y/n): \n"))
                    {
                        isRunning = false;
                        break;
                    }
                }
                header = new string('-', user.Email.Length + 7);
                var userInfo = FormatUserTable(user, header);
                Console.Write(userInfo);
                Console.WriteLine("\n" + header);
                if (UserInputValidation.PromptYesOrNo("Press y to confirm the changes, anything else to discard: "))
                {
                    DbContext.SaveChanges();
                    PrintSuccessMessage("Update was successful.");
                }
                else
                {
                    PrintNotification("Changes were not saved.");
                }
            }
            else
            {
                PrintErrorMessage($"No user with the username {input} exists.");
            }
            PressAnyKeyToContinue();
        }

        private static string FormatUserTable(User user, string header)
        {

            string info = $@"
{header}
Name: {user.FirstName} {user.LastName}
Username: {user.UserName}
Email: {user.Email}
Password: {user.Password}
Subscribed: {user.IsSubscriber}";
            return info;
        }
    }
}
