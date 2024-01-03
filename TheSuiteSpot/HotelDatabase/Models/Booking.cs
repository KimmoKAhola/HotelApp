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
        [Required]
        public bool IsActive { get; set; } = true;
        public int NumberOfExtraBeds { get; set; }
        public string? VoucherCode { get; set; }
    }
}