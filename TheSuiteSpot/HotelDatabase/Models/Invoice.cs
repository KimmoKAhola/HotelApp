using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Services;
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
    }
}