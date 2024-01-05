using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Services.CRUD;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class Room : IEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(10)]
        public string RoomNumber { get; set; } = null!;

        [Required]
        public int RoomSize { get; set; }

        [Range(0, 1E7)]
        public decimal PricePerDay { get; set; }
        public decimal? PricePerExtraBed { get; set; }
        public List<Booking>? Bookings { get; set; } = new List<Booking>();
        public List<Review>? Reviews { get; set; }
        [Required]
        public RoomType RoomType { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return "\n" + RoomCRUD.RoomTemplate(this, new string('-', this.Description.Length));
        }
    }
}
