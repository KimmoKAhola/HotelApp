

namespace InputValidationLibrary
{
    public static class UserInputValidation
    {
        /// <summary>
        /// Promps the user for y (or no). If y then true, else false.
        /// </summary>
        /// <returns></returns>
        public static bool PromptYesOrNo(string msg)
        {
            Console.Write(msg);
            var input = Console.ReadKey(true);
            return input.KeyChar == 'y' || input.KeyChar == 'Y';
        }
        /// <summary>
        /// Let the user choose between two options, one or two.
        /// Return true if 1.
        /// </summary>
        /// <returns></returns>
        public static bool PromptOneOrTwo(string msg)
        {
            Console.Write(msg);
            var input = Console.ReadKey(true);
            return input.KeyChar == '1';
        }

        /// <summary>
        /// Returns a numbered choice between 1 and maximumInput.
        /// Returns -1 if user enters 'e'.
        /// </summary>
        /// <param name="maximumInput"></param>
        /// <returns></returns>
        public static int ReturnNumberChoice(int maximumInput)
        {
            var choice = 0;
            Console.Write($"Enter a number between 1 and {maximumInput}, or press 'e' to exit: ");
            while (true)
            {
                string? input = Console.ReadLine();
                if (input?.ToLower() == "e")
                {
                    return -1;
                }
                if (int.TryParse(input, out choice))
                {
                    if (choice >= 1 && choice <= maximumInput)
                    {
                        return choice;
                    }
                    else
                    {
                        Console.WriteLine($"Please enter a number between 1 and {maximumInput}.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number or 'e' to exit.");
                }
            }
        }

        /// <summary>
        /// Asks for a valid name. May not contain anything other than a letter.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string? AskForValidName(string data, int minimumLength, int maximumLength)
        {
            var value = "";

            while (true)
            {
                Console.Write($"Enter a value for {data},\nat least {minimumLength} and at most {maximumLength} characters without any special characters or numbers\n(type e to exit to main menu): ");
                value = Console.ReadLine();
                if (value.ToLower() == "e")
                {
                    return null;
                }
                if (value.Length >= 2 && value.All(char.IsLetter))
                {
                    Console.WriteLine();
                    return value;
                }
            }
        }
        /// <summary>
        /// Asks for a valid string. May contain letters and characters but no whitespace
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string? AskForValidUsername(string data, int minimumLength, int maximumLength) // , HotelContext ctx
        {
            var value = "";
            while (true)
            {
                Console.Write($"Enter a unique value for {data},\nat least {minimumLength} and at most {maximumLength} characters without whitespace\n(type e to exit to the main menu): ");
                value = Console.ReadLine();

                if (value.Contains("admin", StringComparison.CurrentCultureIgnoreCase))
                {
                    PrintMessages.PrintErrorMessage($"Forbidden word \"{value}\", try again.");
                }
                else if (value.ToLower() == "e")
                {
                    return null;
                }
                else if (value.Length >= minimumLength && !value.Any(char.IsWhiteSpace))
                {
                    Console.WriteLine();
                    return value;
                }
            }
        }
        /// <summary>
        /// Asks for a valid email, but only cares about length. Not characters or domain names.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static string? AskForValidEmail(string data, int minimumLength, int maximumLength)
        {
            var value = "";
            while (true)
            {
                Console.Write($"Enter a unique value for {data},\nat least {minimumLength} and at most {maximumLength} characters without whitespace\nYour email needs to contain @\n(type e to exit to the main menu): ");
                value = Console.ReadLine();

                if (value.Contains("admin", StringComparison.CurrentCultureIgnoreCase))
                {
                    PrintMessages.PrintErrorMessage($"Forbidden word \"{value}\", try again.");
                }
                else if (value.ToLower() == "e")
                {
                    return null;
                }
                else if (value.Length >= minimumLength && !value.Any(char.IsWhiteSpace) && value.Contains('@'))
                {
                    Console.WriteLine();
                    return value;
                }
            }
        }
        /// <summary>
        /// Asks for a valid password. At least 6 characters, and at most 30.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static string? AskForValidPassword(string data, int minimumLength, int maximumLength)
        {
            var value = "";

            while (true)
            {
                Console.Write($"Enter a value for {data},\nat least {minimumLength} and at most {maximumLength} characters without whitespace\n(type e to exit to main menu): ");
                value = Console.ReadLine();
                if (value.ToLower() == "e")
                {
                    return null;
                }
                if (value.Length >= minimumLength && !value.Any(char.IsWhiteSpace))
                {
                    Console.WriteLine();
                    return value;
                }
            }
        }
        /// <summary>
        /// Asks for a general input string. Will truncate its answer to 30 characters. Business rules.
        /// </summary>
        /// <returns></returns>
        public static string? AskForValidInputString()
        {
            var value = "";

            while (true)
            {
                value = Console.ReadLine();
                if (value?.ToLower() == "e" || value == null)
                {
                    return null;
                }
                else
                {
                    Console.WriteLine();
                    if (value.Length > 30)
                    {
                        value = value.Substring(0, 30);
                    }
                    return value;
                }
            }
        }
        public static string AskForValidInputString(string prompt)
        {
            var value = "";
            while (value.Length <= 0 || value == null)
            {
                Console.Write($"Enter your input for the {prompt}: ");
                value = Console.ReadLine();
                if (!(value.Length > 0))
                {
                    PrintMessages.PrintErrorMessage("You can not have an empty input.");
                }
            }
            return value;
        }
        public static string? AskForValidRoomNumber(string data, int minimumLength, int maxiMumLength)
        {
            var value = "";
            while (true)
            {
                Console.Write($"Enter a unique value for {data},\nat least {minimumLength} characters and at most {maxiMumLength} without whitespace\n" +
                    $"Current naming convention is to only use numbers for the rooms,\nbut strings are allowed (type e to exit to main menu): ");
                value = Console.ReadLine();

                if (value.ToLower() == "e")
                {
                    return null;
                }
                else if (value.Length >= minimumLength && value.Length <= maxiMumLength && !value.Any(char.IsWhiteSpace))
                {
                    return value;
                }
            }
        }
        public static decimal? AskForValidNumber(decimal minimumInput, decimal maximumInput, string promptMessage)
        {
            decimal choice;
            while (true)
            {
                Console.WriteLine(promptMessage);
                Console.Write($"Enter a number between {minimumInput} and {maximumInput}, or press 'e' to exit: ");
                string? input = Console.ReadLine();
                if (input?.ToLower() == "e" || input == null)
                {
                    return null;
                }
                if (decimal.TryParse(input, out choice))
                {
                    if (choice >= minimumInput && choice <= maximumInput)
                    {
                        return choice;
                    }
                    else
                    {
                        PrintMessages.PrintErrorMessage($"Please enter a number between {minimumInput} and {maximumInput}.");
                    }
                }
                else
                {
                    PrintMessages.PrintErrorMessage("Invalid input.");
                }
            }
        }
        public static DateTime? AskForValidDate(DateTime limit)
        {
            while (true)
            {
                Console.Write($"Enter a date (yyyy-mm-dd) later than {limit.ToShortDateString()}, or press e to exit: ");
                var input = Console.ReadLine();
                if (input.ToLower() == "e")
                {
                    return null;
                }

                if (DateTime.TryParse(input, out var date) && date > limit)
                {
                    PrintMessages.PrintNotification($"You chose {date.ToShortDateString()}.");
                    return date + new TimeSpan(8, 0, 0);
                }
                else if (date < limit)
                {
                    PrintMessages.PrintErrorMessage($"Please choose a date after {limit.ToShortDateString()}");
                }
                else
                {
                    PrintMessages.PrintErrorMessage("Please enter a date on the correct format (yyyy-mm-dd).");
                }
            }
        }

        public static DateTime? AskForValidEndDate(DateTime startDate, DateTime endDate)
        {
            while (true)
            {
                Console.Write($"Enter a date (yyyy-mm-dd) at latest {endDate.ToShortDateString()}," +
                    $"\n and after {startDate.ToShortDateString()}" +
                    $", or press e to exit: ");
                var input = Console.ReadLine();
                if (input.ToLower() == "e")
                {
                    return null;
                }

                if (DateTime.TryParse(input, out var chosenDate) && chosenDate > startDate && chosenDate < endDate)
                {
                    PrintMessages.PrintNotification($"You chose {chosenDate.ToShortDateString()}.");
                    return chosenDate + new TimeSpan(16, 0, 0);
                }
                else if (chosenDate < startDate)
                {
                    PrintMessages.PrintErrorMessage($"Please choose a date after {startDate.ToShortDateString()}");
                }
                else if (chosenDate > startDate.AddDays(10))
                {
                    PrintMessages.PrintErrorMessage($"Please choose a date earlier than {endDate.ToShortDateString()}");
                }
                else
                {
                    PrintMessages.PrintErrorMessage("Please enter a date on the correct format (yyyy-mm-dd).");
                }
            }
        }
        public static int MenuValidation(Dictionary<int, string> choices, string promptMessage)
        {
            var maxValue = choices.Count;
            foreach (var kvp in choices)
            {
                Console.WriteLine($"{kvp.Key}. {kvp.Value}");
            }

            Console.Write(promptMessage);
            var choice = ReturnNumberChoice(maxValue);
            return choice;
        }
        public static int MenuValidation<T>(List<T> choices, string promptMessage)
        {
            var maxValue = choices.Count;
            for (int i = 0; i < choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {choices[i]}");
            }
            Console.Write(promptMessage);
            var choice = ReturnNumberChoice(maxValue);
            return choice;
        }
    }
}