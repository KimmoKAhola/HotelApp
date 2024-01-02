using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class NewsletterIssueSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void SeedData()
        {
            if (!DbContext.Message.Any())
            {
                var subscriptionNewsletter = DbContext.MessageType.Where(n => n.Name == SystemMessageTypes.Subscription.ToString()).First();
                var systemNewsletter = DbContext.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First();
                var allSubscribers = DbContext.User.Where(u => u.IsSubscriber && !u.IsAdmin);
                foreach (var subscriber in allSubscribers)
                {
                    var userTerms = new SystemMessage
                    {
                        Topic = "Terms of agreement",
                        Content = "Welcome to The Suite Spot! By booking a stay with us, you agree to indulge in luxury and comfort.\n" +
                        "Please take a moment to familiarize yourself with our terms of agreement, ensuring a delightful experience for both you and our team.\n" +
                        "Your relaxation journey begins with us at The Suite Spot.",
                        MessageType = systemNewsletter,
                    };
                    DbContext.Message.Add(userTerms);
                    DbContext.SaveChanges();
                }

                allSubscribers = DbContext.User.Where(u => u.IsSubscriber && u.IsActive && !u.IsAdmin);
                foreach (var subscriber in allSubscribers)
                {
                    var voucher = Voucher.GenerateVoucherCode(20m);
                    DbContext.Add(voucher);
                    var subscriptionLetter = new SystemMessage
                    {
                        Topic = "Voucher to our subscriber",
                        Content = "Welcome to the suite spot. We offer a free voucher to our subscribers as a thank you for trusting us.",
                        MessageType = subscriptionNewsletter,
                        Voucher = voucher
                    };
                    DbContext.Message.Add(subscriptionLetter);
                }
                DbContext.SaveChanges();
                var allIssues = DbContext.Message.Where(v => v.Voucher != null);
                foreach (var issue in allIssues)
                {
                    //UserInbox.SendNewsLetterWithVoucher(issue.User, DbContext, issue);
                }
                DbContext.SaveChanges();
            }
        }
    }
}
