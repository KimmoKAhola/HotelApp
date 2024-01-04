using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary.HotelDatabase.DatabaseConfiguration;
using DatabaseLibrary.Interfaces;

namespace DatabaseLibrary.HotelDatabase.Models
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

        /// <summary>
        /// Move me to a service class
        /// </summary>
        /// <param name="discountPercentage"></param>
        /// <returns></returns>
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
