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
        public bool IsExpired { get; set; } = false;
        [Required]
        public decimal DiscountPercentage { get; set; }
        public override string ToString()
        {
            return $"Quick voucher summary:" +
                $"\nCode: {VoucherCode}" +
                $"\nExpiration date: {ExpiryDate}" +
                $"\nIs expired: {IsExpired}" +
                $"\nDiscount percentage: {DiscountPercentage} %";
        }
    }
}
