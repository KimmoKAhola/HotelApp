using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class Voucher : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string VoucherCode { get; set; } = null!;
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(30);
        [Required]
        public decimal DiscountPercentage { get; set; }

        public static Voucher GenerateVoucherCode(decimal discountPercentage)
        {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString("N");

            string randomString = guidString[..5];

            var voucher = new Voucher
            {
                VoucherCode = randomString,
                DiscountPercentage = discountPercentage
            };
            return voucher;
        }
    }
}
