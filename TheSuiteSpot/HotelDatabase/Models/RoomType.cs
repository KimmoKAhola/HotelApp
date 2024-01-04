using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public enum SuiteTypes
    {
        Junior = 1,
        Executive,
        Deluxe,
        Royal,
    }
    public class RoomType : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string SuiteName { get; set; } = null!;

        [Range(0, 2)]
        public int NumberOfExtraBeds { get; set; }

        public bool IsDoubleRoom { get; set; } = false;

        public override string ToString()
        {
            return $"\nType of room: {SuiteName}\nIs it a double room: {IsDoubleRoom}" +
                $"\nNumber of extra beds available: {NumberOfExtraBeds}\n";
        }
    }
}
