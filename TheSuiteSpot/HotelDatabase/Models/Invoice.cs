using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class Invoice : IEntity
    {
        public int Id { get; set; }
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(10);
        [Required]
        [Range(0, 1E9)]
        public decimal Amount { get; set; }
        public string? InvoiceDescription { get; set; }
        [Required]
        public Booking Booking { get; set; } = null!;
        [Required]
        public bool IsPaid { get; set; } = false;
        [Required]
        public bool IsActive { get; set; } = true;

        public static decimal CalculateAdditionalCost(Invoice invoice)
        {
            invoice.Amount += invoice.Booking.NumberOfExtraBeds * (decimal)invoice.Booking.Room.PricePerExtraBed;
            return invoice.Amount;
        }

        public static string GenerateInvoice(Invoice invoice)
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
            invoiceBuilder.AppendLine($"\nTotal Amount: {totalAmount:C}");

            invoiceBuilder.AppendLine("\nThank you for choosing The Suite Spot! We appreciate your business.");
            invoiceBuilder.AppendLine($"For any inquiries, please contact us at {CurrentUser.Instance.User.Email}");

            return invoiceBuilder.ToString();
        }


        public override string ToString()
        {
            return $"Invoice was created at: {DateCreated}\n" +
                $"\nBooking: {Booking}" +
                $"\n{InvoiceDescription}";
        }
    }
}