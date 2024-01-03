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

        /// <summary>
        /// Move me to a service class
        /// </summary>
        /// <param name="suiteName"></param>
        /// <returns></returns>
        public static int GetMaxAmountOfExtraGuests(string suiteName)
        {
            int numberOfAdditionalGuests = 0;
            switch (suiteName)
            {
                case "Junior":
                    numberOfAdditionalGuests = 0;
                    break;
                case "Executive":
                    numberOfAdditionalGuests = 1;
                    break;
                case "Deluxe":
                    numberOfAdditionalGuests = 3;
                    break;
                case "Royal":
                    numberOfAdditionalGuests = 5;
                    break;
            }
            return numberOfAdditionalGuests;
        }

        public override string ToString()
        {
            return $"\nType of room: {SuiteName}\nIs it a double room: {IsDoubleRoom}" +
                $"\nNumber of extra beds available: {NumberOfExtraBeds}\n";
        }
    }
}
