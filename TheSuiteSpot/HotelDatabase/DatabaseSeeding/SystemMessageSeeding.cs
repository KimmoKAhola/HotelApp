using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.HotelDatabase.Services;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class SystemMessageSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void SeedData()
        {
            if (DbContext.SystemMessage.Count() < 500)
            {
                var subscriptionNewsletter = DbContext.SystemMessageType.Where(n => n.Name == SystemMessageTypes.Subscription.ToString()).First();
                var systemNewsletter = DbContext.SystemMessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First();
                var allSubscribers = DbContext.User.Include(u => u.UserInbox).Where(u => u.IsSubscriber && !u.IsAdmin);
                foreach (var subscriber in allSubscribers)
                {
                    var userTerms = new SystemMessage
                    {
                        Topic = "Terms of agreement",
                        Sender = DbContext.UserRole.Where(u => u.RoleName == UserRoles.System.ToString()).First().RoleName,
                        Content = "Welcome to The Suite Spot! By booking a stay with us, you agree to indulge in luxury and comfort.\n" +
                        "Please take a moment to familiarize yourself with our terms of agreement, ensuring a delightful experience for both you and our team.\n" +
                        "Your relaxation journey begins with us at The Suite Spot.",
                        MessageType = systemNewsletter,
                    };
                    DbContext.SystemMessage.Add(userTerms);
                    subscriber.UserInbox.Messages.Add(userTerms);
                    DbContext.SaveChanges();
                }

                allSubscribers = DbContext.User.Include(u => u.UserInbox).Where(u => u.IsSubscriber && u.IsActive && !u.IsAdmin);
                foreach (var subscriber in allSubscribers)
                {
                    var voucher = VoucherServices.GenerateVoucherCode(20m);
                    DbContext.Add(voucher);
                    var subscriptionLetter = new SystemMessage
                    {
                        Topic = "Voucher to our subscriber",
                        Sender = DbContext.UserRole.Where(u => u.RoleName == UserRoles.System.ToString()).First().RoleName,
                        Content = "Welcome to the suite spot. We offer a free voucher to our subscribers as a thank you for trusting us.",
                        MessageType = subscriptionNewsletter,
                        Voucher = voucher
                    };
                    DbContext.SystemMessage.Add(subscriptionLetter);
                    subscriber.UserInbox.Messages.Add(subscriptionLetter);
                }
                DbContext.SaveChanges();
            }
        }
    }
}
