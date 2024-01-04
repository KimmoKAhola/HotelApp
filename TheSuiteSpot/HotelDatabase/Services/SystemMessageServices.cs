using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using Microsoft.EntityFrameworkCore;
using TheSuiteSpot.HotelDatabase.Services.CRUD;

namespace TheSuiteSpot.HotelDatabase.Services
{
    public class SystemMessageServices
    {
        public static void SendRewardMessage(HotelContext ctx, decimal discountPercentage, User receiver)
        {
            var randomString = VoucherServices.GenerateVoucherCode(discountPercentage);
            var voucher = new Voucher
            {
                VoucherCode = $"CODE-{randomString.VoucherCode}",
                DiscountPercentage = discountPercentage,
            };
            ctx.Add(voucher);
            ctx.SaveChanges();

            var chosenUser = ctx.User.Include(u => u.UserInbox).Where(u => u.Id == receiver.Id).First();
            var reward = new SystemMessage
            {
                Topic = "A reward to our most valued customer.",
                Sender = ctx.User.Where(u => u.UserName == "System").First().UserName,
                Content = "Thank you, dear customer, for your patronage. As a reward, we will award you with a voucher.",
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.Reward.ToString()).First(),
            };
            reward.Voucher = voucher;
            chosenUser.UserInbox.Messages.Add(reward);
            ctx.SaveChanges();
        }
        public static void SendSystemMessage(HotelContext ctx, User receiver, string topic, string content)
        {
            var chosenUser = ctx.User.Where(u => u.Id == receiver.Id).First();
            var newsletter = new SystemMessage
            {
                Topic = topic,
                Sender = ctx.User.Where(u => u.UserName == "System").First().UserName,
                Content = content,
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First(),
            };

            receiver.UserInbox.Messages.Add(newsletter);
            ctx.SaveChanges();
        }

        public static void SendMessageBetweenUsers(HotelContext ctx, User sender, User receiver, string topic, string content)
        {
            var message = new SystemMessage
            {
                Topic = topic,
                Sender = sender.UserName,
                Content = content,
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.UserToUser.ToString()).First(),
            };

            receiver.UserInbox.Messages.Add(message);
            ctx.SaveChanges();
        }

        public static void SendBookingConfirmationMessage(HotelContext ctx, User receiver, Booking booking)
        {

            var message = new SystemMessage
            {
                Topic = "Booking confirmation",
                Sender = ctx.User.Where(u => u.UserName == "System").First().UserName,
                Content = BookingCRUD.FormatBooking(booking),
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First(),
            };
            receiver.UserInbox.Messages.Add(message);
            ctx.SaveChanges();
        }

        public static void SendInvoiceMessage(HotelContext ctx, User receiver, Invoice invoice, Booking booking)
        {
            var message = new SystemMessage
            {
                Topic = "Invoice",
                Sender = ctx.User.Where(u => u.UserName == "System").First().UserName,
                Content = InvoiceCRUD.InvoiceTemplate(invoice, receiver, booking),
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First(),
            };
            receiver.UserInbox.Messages.Add(message);
            ctx.SaveChanges();
        }

        public static void SendCreatedUserMessage(User receiver, HotelContext ctx)
        {
            var message = new SystemMessage
            {
                Topic = "New account created",
                Sender = ctx.User.Where(u => u.UserName == "System").First().UserName,
                Content = ($"Your account has been created. Welcome to The Suite Spot! \nYour account information is: " +
                $"\nUsername: {receiver.UserName}" +
                $"\nEmail: {receiver.Email}" +
                $"\nPassword: {receiver.Password}"),
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First(),
            };
            receiver.UserInbox.Messages.Add(message);
            ctx.SaveChanges();
        }
    }
}
