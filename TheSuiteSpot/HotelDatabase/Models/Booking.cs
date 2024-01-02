using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class Booking : IEntity
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now.Date + new TimeSpan(8, 0, 0);
        public DateTime EndDate { get; set; }
        public User User { get; set; } = null!;
        public Room Room { get; set; } = null!;
        public bool HasActiveInvoice { get; set; } = false;
        [Required]
        public bool IsActive { get; set; } = true;

        public int NumberOfExtraBeds { get; set; }

        public string? VoucherCode { get; set; }

        public static string GenerateBooking(User receiver)
        {
            return "Your booking has been created with the booking id[1] Your booking is between 2023 - 12 - 30 08:00:00 - 2024 - 01 - 04 16:00:00";
        }

        public override string ToString()
        {
            return $"Start: {StartDate}" +
                $"\nEnd: {EndDate}" +
                $"\nUser: {User}";
        }
    }
}