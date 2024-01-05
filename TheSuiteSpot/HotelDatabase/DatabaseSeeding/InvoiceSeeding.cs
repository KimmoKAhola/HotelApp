using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.HotelDatabase.Services;
using TheSuiteSpot.HotelDatabase.Services.CRUD;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class InvoiceSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void SeedData()
        {
            using (var ctx = new HotelContext())
            {

                if (!ctx.Invoice.Any())
                {
                    var allUsersWithBookings = ctx.Booking
                        .Include(u => u.User)
                        .ThenInclude(u => u.UserInbox)
                        .Include(r => r.Room)
                        .ThenInclude(rt => rt.RoomType).ToList();

                    foreach (var booking in allUsersWithBookings)
                    {
                        var currentUser = booking.User;
                        var admin = ctx.User.Where(u => u.IsAdmin).First();

                        var numberOfDays = (int)(booking.EndDate - booking.StartDate).TotalDays;

                        var pricePerDay = booking.Room.PricePerDay;
                        var totalAmount = pricePerDay * numberOfDays;
                        var invoice = new Invoice
                        {
                            DateCreated = booking.StartDate,
                            DueDate = booking.StartDate.AddDays(10),
                            Amount = totalAmount,
                            Booking = booking,
                        };
                        if (invoice.DueDate < DateTime.Today.AddDays(10) && invoice.Booking.User.UserName != "Richard")
                        {
                            invoice.IsPaid = true;
                        }
                        var invoiceDescription = InvoiceCRUD.InvoiceTemplate(invoice, currentUser, booking);
                        invoice.InvoiceDescription = invoiceDescription;
                        ctx.Invoice.Add(invoice);
                        ctx.SaveChanges();
                        SystemMessageServices.SendInvoiceMessage(ctx, currentUser, invoice, booking);
                    }
                }
                ctx.SaveChanges();
            }
        }
    }
}