using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputValidationLibrary
{
    public static class PrintMessages
    {
        public static void PrintErrorMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintSuccessMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PrintNotification(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void PressAnyKeyToContinue()
        {
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
