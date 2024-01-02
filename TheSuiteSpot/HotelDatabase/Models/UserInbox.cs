using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class UserInbox : IEntity
    {
        public UserInbox()
        {

        }
        public UserInbox(HotelContext dbContext)
        {
            DbContext = dbContext;
        }
        public HotelContext DbContext { get; set; }
        public int Id { get; set; }
        public List<SystemMessage> Messages { get; set; } = new List<SystemMessage>();



        /// <summary>
        /// Move to a service class later on
        /// </summary>
        //public static void SendCreatedUserMessage(User receiver, HotelContext ctx)
        //{
        //    var message = new UserInbox
        //    {
        //        Topic = "New account created",
        //        Sender = ctx.User.Where(u => u.UserName == UserRoles.System.ToString()).First().UserName,
        //        //Receiver = receiver,
        //        MessageText = ($"Your account has been created. Welcome to The Suite Spot! \nYour account information is: " +
        //        $"\nUsername: {receiver.UserName}" +
        //        $"\nEmail: {receiver.Email}" +
        //        $"\nPassword: {receiver.Password}"),
        //    };
        //    ctx.Inbox.Add(message);
        //    ctx.SaveChanges();
        //}
        //public static void SendCreatedBookingMessage(User receiver, HotelContext ctx)
        //{
        //    var bookingText = Booking.GenerateBooking(receiver);
        //    var message = new UserInbox
        //    {
        //        Topic = "Booking confirmation",
        //        Sender = ctx.User.Where(u => u.UserName == UserRoles.System.ToString()).First().UserName,
        //        //Receiver = receiver,
        //        MessageText = bookingText
        //    };
        //    //receiver.UserInbox.Add(message);
        //    //ctx.SaveChanges();
        //    //CurrentUser.Instance.User.UserInbox.Add(message);
        //    //ctx.SaveChanges();
        //}
        //public static void SendCreatedInvoiceMessage(User receiver, HotelContext ctx, Invoice invoice)
        //{
        //    var invoiceText = Invoice.GenerateInvoice(invoice);
        //    var messageToUser = new UserInbox
        //    {
        //        Topic = "New invoice",
        //        Sender = ctx.User.Where(u => u.UserName == UserRoles.System.ToString()).First().UserName,
        //        //Receiver = receiver,
        //        MessageText = invoiceText,
        //    };
        //    //receiver.UserInbox.Add(messageToUser);
        //    //ctx.SaveChanges();
        //}

        //public static void SendDeletedUserConfirmation(User deletedUser)
        //{
        //    //using (var ctx = new HotelContext())
        //    //{
        //    //    var message = new UserMessage
        //    //    {
        //    //        Sender = CurrentUser.Instance.User,
        //    //        Receiver = CurrentUser.Instance.User,
        //    //        MessageText = ($"You have deleted the user: {deletedUser.UserName}")
        //    //    };
        //    //    ctx.UserMessage.Add(message);
        //    //    ctx.SaveChanges();
        //    //}
        //}
    }
}
