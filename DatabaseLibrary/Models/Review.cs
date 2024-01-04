using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary.Interfaces;

namespace DatabaseLibrary.HotelDatabase.Models
{
    public class Review : IEntity
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string UserName { get; set; } = "Anonymous user";
        [Required]
        [MaxLength(3000)]
        public string ReviewText { get; set; } = null!;
        [Required]
        public DateOnly DateOfReview { get; set; } = DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
        [Required]
        [Range(1, 100)]
        public byte StarsGiven { get; set; }
        [Required]
        public Room Room { get; set; } = null!;
    }
}
