using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Models;

namespace TheSuiteSpot.HotelDatabase.Services
{
    public class InvoiceServices
    {
        /// <summary>
        /// Move me to a service class
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public static decimal CalculateAdditionalCost(Invoice invoice)
        {
            invoice.Amount += invoice.Booking.NumberOfExtraBeds * (decimal)invoice.Booking.Room.PricePerExtraBed;
            return invoice.Amount;
        }

        public static void CalculateNewInvoice(Booking oldBooking, Booking newBooking)
        {

        }


        /// <summary>
        /// Move me to a service class
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public static string GenerateInvoice(Invoice invoice, Voucher? voucher)
        {
            StringBuilder invoiceBuilder = new StringBuilder();

            invoiceBuilder.AppendLine($"[{invoice.Booking.Room.RoomType.SuiteName}] - {invoice.Booking.StartDate}");

            invoiceBuilder.AppendLine($"\nInvoice Due Date: {invoice.DueDate}");

            invoiceBuilder.AppendLine($"\nBill To:\n{invoice.Booking.User.FirstName} {invoice.Booking.User.LastName}");

            invoiceBuilder.AppendLine("\nDescription:");
            invoiceBuilder.AppendLine($"Room Reservation: {invoice.Booking.Room.RoomNumber}");
            invoiceBuilder.AppendLine($"Check-in Date: {invoice.Booking.StartDate}");
            invoiceBuilder.AppendLine($"Check-out Date: {invoice.Booking.EndDate}");
            invoiceBuilder.AppendLine($"Number of Nights: {invoice.Booking.EndDate - invoice.Booking.StartDate}");
            invoiceBuilder.AppendLine($"Rate per Night: {invoice.Booking.Room.PricePerDay:C2}");

            if (invoice.Booking.NumberOfExtraBeds > 0)
            {
                decimal extraCost = (decimal)invoice.Booking.Room.PricePerExtraBed;
                invoiceBuilder.AppendLine($"Extra Beds: {extraCost:C2}");
                invoice.Booking.Room.PricePerDay += extraCost;
            }

            decimal totalAmount = invoice.Booking.Room.PricePerDay * (invoice.Booking.EndDate - invoice.Booking.StartDate).Days;
            if (invoice.Booking.VoucherCode != null) { totalAmount = invoice.Amount; }
            invoiceBuilder.AppendLine($"\nTotal Amount: {totalAmount:C}");
            if (invoice.Booking.VoucherCode != null) { invoiceBuilder.AppendLine($"Discount of {voucher.DiscountPercentage} % has been applied."); }
            invoiceBuilder.AppendLine("\nThank you for choosing The Suite Spot! We appreciate your business.");
            invoiceBuilder.AppendLine($"For any inquiries, please contact us at {CurrentUser.Instance.User.Email}");

            return invoiceBuilder.ToString();
        }
    }
}
