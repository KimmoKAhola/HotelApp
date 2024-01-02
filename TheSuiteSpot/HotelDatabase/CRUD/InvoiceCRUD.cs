using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
//using TheSuiteSpot.HotelDatabase.InputHelpers;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;
//using static TheSuiteSpot.HotelDatabase.InputHelpers.PrintMessages;
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
            {3, "Invoice Description"},
            {4, "Payment received"},
            {5, "Delete invoice"},
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
            if (UserInputValidation.PromptOneOrTwo("Press 1 to view paid invoices, anything else to view unpaid invoices: "))
            {
                invoices = invoices = ctx.Invoice
                    .Where(i => i.IsPaid)
                    .Include(b => b.Booking)
                    .ThenInclude(r => r.Room)
                    .Include(b => b.Booking)
                    .ThenInclude(u => u.User)
                    .ToList();
            }
            else
            {
                invoices = invoices = ctx.Invoice
                    .Where(i => !i.IsPaid)
                    .Include(b => b.Booking)
                    .ThenInclude(r => r.Room)
                    .Include(b => b.Booking)
                    .ThenInclude(u => u.User)
                    .ToList();
            }

            foreach (var invoice in invoices)
            {
                Console.Clear();
                var info = InvoiceTemplate(invoice, invoice.Booking.User, invoice.Booking);
                Console.WriteLine(info);
                if (!invoice.IsPaid)
                    if (UserInputValidation.PromptYesOrNo("Click y to change this invoice status to paid: "))
                    {
                        invoice.IsPaid = true;
                        PrintNotification("Status has been changed to paid.");
                    }
                if (!UserInputValidation.PromptYesOrNo("Press y to view another, anything else to exit: "))
                {
                    break;
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
            throw new NotImplementedException();
        }

        public void GeneralSearch(HotelContext ctx)
        {
            throw new NotImplementedException();
        }

        public void Update(HotelContext ctx)
        {
            Console.WriteLine("You will now update an invoice of your choice.");
            var invoice = GetInvoice(ctx);

            Console.Clear();
            Console.WriteLine($"Your chosen invoice is:\n{invoice}");
            ChangeInvoiceProperty();
            PressAnyKeyToContinue();
        }

        private void ChangeInvoiceProperty()
        {
            foreach (var item in _modelProperties)
            {
                Console.WriteLine($"{item.Key}. {item.Value}");
            }
            Console.Write("Pick a property you wish to change: ");
            PressAnyKeyToContinue();
        }

        public static string InvoiceTemplate(Invoice invoice, User user, Booking booking)
        {
            string length = "Thank you for choosing The Suite Spot! We appreciate your business.";
            var header = new string('-', length.Length);
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

        public static string InvoicePenaltyFeeTemplate(Invoice invoice, User user, string message)
        {
            return $@"
[The Suite spot] - {invoice.DateCreated} - PenaltyFee

Invoice Due Date: {invoice.DueDate}

Bill To:
{user.FirstName} {user.LastName}

Description:
{message}

Total Amount: {invoice.Amount:C2}

For any inquiries, please contact us at {CurrentUser.Instance.User.Email}.
";
        }
    }
}
