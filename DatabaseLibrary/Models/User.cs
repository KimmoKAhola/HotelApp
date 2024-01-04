using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(30), MinLength(6)]
        public string Password { get; set; } = null!;
        [Required]
        [StringLength(30, MinimumLength = 6)]
        public string UserName { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        [Required]
        public UserInbox UserInbox { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; } = true;
        public DateTime LastLogin { get; set; } = DateTime.Now;
        public bool IsAdmin { get; set; } = false;
        public UserRole UserRole { get; set; } = null!;
        public bool IsSubscriber { get; set; } = true;
    }
}
