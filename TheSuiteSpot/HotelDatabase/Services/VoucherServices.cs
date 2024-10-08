﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Models;

namespace TheSuiteSpot.HotelDatabase.Services
{
    public class VoucherServices
    {
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