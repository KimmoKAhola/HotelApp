using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
using InputValidationLibrary;
using static InputValidationLibrary.PrintMessages;

namespace TheSuiteSpot.HotelDatabase.CRUD
{
    public class InvoiceCRUD(HotelContext dbContext) : ICRUD
    {
        public HotelContext DbContext { get; set; } = dbContext;

        private Dictionary<int, string> _modelProperties = new Dictionary<int, string>()
        {
            {1, "Due Date"},
            {2, "Amount"},
            {3, "Payment status"},
        };

        public void Create(HotelContext ctx)
        {
            //Do not implement
        }

        public void SoftDelete(HotelContext ctx)
        {

        }


        public void ReadAll(HotelContext ctx)
        {
            Console.Clear();
            List<Invoice> invoices;

            invoices = ctx.Invoice
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.DueDate)
                .Include(b => b.Booking)
                .ThenInclude(r => r.Room)
                .Include(b => b.Booking)
                .ThenInclude(u => u.User)
                .ThenInclude(u => u.UserInbox)
                .ToList();

            PrintNotification($"There are currently {invoices.Count} unpaid invoices.");
            if (UserInputValidation.PromptYesOrNo("Press y to handle unpaid invoices, anything else to skip: "))
            {
                foreach (var invoice in invoices)
                {
                    Console.Clear();
                    var info = InvoiceTemplate(invoice, invoice.Booking.User, invoice.Booking);
                    Console.WriteLine(info);
                    if (!invoice.IsPaid)
                        if (UserInputValidation.PromptYesOrNo("Click y to change this invoice status to paid, anything else to continue: "))
                        {
                            invoice.IsPaid = true;
                            ctx.SaveChanges();
                            var content = $"Dear sir/mam, we have received your payment of {invoice.Amount:C2}. Thank you for your patronage.";
                            SystemMessage.SendSystemMessage(ctx, invoice.Booking.User, "Payment confirmed", content);
                            PrintNotification("Status has been changed to paid.");
                        }
                        else
                        {
                            PrintNotification("You chose no");
                        }
                    if (!UserInputValidation.PromptYesOrNo("Press y to view another, anything else to exit: "))
                    {
                        PrintNotification("You chose to exit");
                        break;
                    }
                }
            }
            ctx.SaveChanges();
            PressAnyKeyToContinue();
        }

        private static Invoice GetInvoice(HotelContext ctx)
        {
            List<Invoice> invoices;
            if (UserInputValidation.PromptOneOrTwo("Press 1 to view paid invoices, anything else to view unpaid invoices: "))
            {
                invoices = ctx.Invoice.Where(i => i.IsPaid).ToList();
            }
            else
            {
                invoices = ctx.Invoice.Where(i => !i.IsPaid).ToList();
            }
            foreach (var invoice in invoices)
            {
                Console.WriteLine($"-------------\nindex {invoice.Id}\n" + invoice + "---------------");
            }
            Console.Write("Choose an invoice index: ");
            var choice = Convert.ToInt32(Console.ReadLine());
            return invoices[choice - 1];
        }

        public void ExactSearch(HotelContext ctx)
        {
            //Is it needed?
        }

        public void GeneralSearch(HotelContext ctx)
        {
            Console.Clear();
            PrintNotification("You can pick a search term and receive all invoices where the user contains that search term.\nTry to be specific.");
            var searchInput = UserInputValidation.AskForValidInputString("");
            var searchResult = ctx.Invoice
                .Where(i => i.IsActive)
                .OrderBy(i => i.Id)
                .Include(b => b.Booking)
                .ThenInclude(r => r.Room)
                .Include(u => u.Booking.User)
                .Where(i => i.Booking.User.UserName.Contains(searchInput)
                || i.Booking.User.Email.Contains(searchInput)
                || i.Booking.User.FirstName.Contains(searchInput)
                || i.Booking.User.LastName.Contains(searchInput));

            foreach (var invoice in searchResult)
            {
                var stringLength = invoice.Booking.StartDate.ToString().Length;
                var divider = new string('-', (int)(stringLength * 2.5));
                Console.WriteLine(divider);
                Console.WriteLine($"Invoice with id: {invoice.Id} with due date: {invoice.DueDate}");
                Console.WriteLine($"Room booked: {invoice.Booking.Room.RoomNumber}");
                Console.WriteLine($"Booking dates: [{invoice.Booking.StartDate}] - [{invoice.Booking.EndDate}]");
                Console.WriteLine($"Username: {invoice.Booking.User.UserName}");
                Console.WriteLine($"Name: {invoice.Booking.User.FirstName} {invoice.Booking.User.LastName}");
                Console.WriteLine($"Email: {invoice.Booking.User.Email}");
                Console.WriteLine($"Invoice amount: {invoice.Amount:C2}");
                Console.WriteLine($"Has been paid: {invoice.IsPaid}");
                if (invoice == searchResult.Last())
                {
                    Console.WriteLine(divider);
                }
            }
            PressAnyKeyToContinue();
        }

        public void Update(HotelContext ctx)
        {
            Console.Clear();
            PrintNotification("You can only update invoices that have not yet been paid.");

            var unpaidInvoices = ctx.Invoice
                .Include(b => b.Booking)
                .ThenInclude(u => u.User)
                .Include(r => r.Booking.Room)
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.Id).ToList();
            if (unpaidInvoices.Count > 0)
            {
                PrintNotification("These are your unpaid invoices: ");
                List<int> invoiceId = new List<int>();
                foreach (var invoice in unpaidInvoices)
                {
                    invoiceId.Add(invoice.Id);
                    var stringLength = invoice.DueDate.ToString().Length;
                    var divider = new string('-', (int)(stringLength * 2.5));
                    Console.WriteLine(divider);
                    Console.WriteLine($"Invoice with id: {invoice.Id}");
                    Console.WriteLine($"Invoice due date: {invoice.DueDate}");
                    Console.WriteLine($"Amount: {invoice.Amount:C2}");
                    if (invoice == unpaidInvoices.Last())
                    {
                        Console.WriteLine(divider);
                    }
                }
                PrintNotification("\nHere are the invoice Ids that you can change.");
                var choice = UserInputValidation.MenuValidation(invoiceId, "Choose an invoice id. ");
                var chosenInvoice = unpaidInvoices[choice - 1];
                PrintNotification($"You chose the invoice with id {unpaidInvoices[choice - 1].Id}");
                ChangeInvoiceProperty(chosenInvoice, ctx);
            }
            else
            {
                PrintNotification("There are no unpaid invoices.");
            }
            PressAnyKeyToContinue();
        }

        private void ChangeInvoiceProperty(Invoice invoice, HotelContext ctx)
        {
            var choice = UserInputValidation.MenuValidation(_modelProperties, "Choose which property you want to change. ");

            switch (choice)
            {
                case 1:
                    int? days = (int?)UserInputValidation.AskForValidNumber(1, 30, "You can add additional payment days to the invoice.");
                    if (days == null) { return; }
                    invoice.DueDate = invoice.DueDate.AddDays((double)days);
                    break;
                case 2:
                    decimal? amount = UserInputValidation.AskForValidNumber(0, Convert.ToDecimal(1E9), "You can change the invoice amount.");
                    if (amount == null) { return; }
                    invoice.Amount = (decimal)amount;
                    break;
                case 3:
                    if (UserInputValidation.PromptYesOrNo("Press y to flip the payment status, anything else to skip: "))
                    {
                        invoice.IsPaid = true;
                        PrintSuccessMessage("Invoice is now paid.");
                    }
                    break;
            }
            var info = InvoiceTemplate(invoice, invoice.Booking.User, invoice.Booking);
            Console.WriteLine(info);
            if (UserInputValidation.PromptYesOrNo("Press y to confirm the changes, anything else to discard: "))
            {
                ctx.SaveChanges();
                PrintSuccessMessage("Your changes has been applied.");
            }
            else
            {
                PrintNotification("Your changes has been discarded.");
            }
        }

        public static string InvoiceTemplate(Invoice invoice, User user, Booking booking)
        {
            string length = "Thank you for choosing The Suite Spot! We appreciate your business.";
            var header = new string('-', length.Length);
            if (booking.VoucherCode == null)
            {
                return $@"{header}
[The Suite spot] - {invoice.DateCreated}

Invoice Due Date: {invoice.DueDate}

Bill To:
{user.FirstName} {user.LastName}

Description:
Room Reservation: {booking.Room.RoomNumber}
Check-in Date: {booking.StartDate}
Check-out Date: {booking.EndDate}
Number of Nights: {(booking.EndDate - booking.StartDate).Days}
Rate per Night: {booking.Room.PricePerDay:C2}

Total Amount: {invoice.Amount:C2}

Thank you for choosing The Suite Spot! We appreciate your business.
For any inquiries, please contact us at {CurrentUser.Instance.User.Email}.
{header}";
            }
            else
            {
                return $@"{header}
[The Suite spot] - {invoice.DateCreated}

Invoice Due Date: {invoice.DueDate}

Bill To:
{user.FirstName} {user.LastName}

Description:
Room Reservation: {booking.Room.RoomNumber}
Check-in Date: {booking.StartDate}
Check-out Date: {booking.EndDate}
Number of Nights: {(booking.EndDate - booking.StartDate).Days}
Rate per Night: {booking.Room.PricePerDay:C2}

Total Amount: {invoice.Amount:C2} - Discounted!

Thank you for choosing The Suite Spot! We appreciate your business.
For any inquiries, please contact us at {CurrentUser.Instance.User.Email}.
{header}";
            }
        }
    }
}
