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
    public class SystemMessage : IEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Topic { get; set; } = null!;

        [MaxLength(5000)]
        public string Content { get; set; } = null!;
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public Voucher? Voucher { get; set; }
        public SystemMessageType MessageType { get; set; } = null!;
        public static void GenerateNewsLetterWithReward(HotelContext ctx, decimal discountPercentage, User user)
        {
            var randomString = Voucher.GenerateVoucherCode(discountPercentage);
            var voucher = new Voucher
            {
                VoucherCode = $"CODE-{randomString.VoucherCode}",
                DiscountPercentage = discountPercentage,
            };
            ctx.Add(voucher);
            ctx.SaveChanges();

            var chosenUser = ctx.User.Where(u => u.Id == user.Id).First();
            var rewardIssue = new SystemMessage
            {
                Topic = "A reward to our most valued customer.",
                Content = "Thank you, dear customer, for your patronage. As a reward, we will award you with a voucher.",
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.Reward.ToString()).First(),
            };
            rewardIssue.Voucher = voucher;
            ctx.Add(rewardIssue);
            ctx.SaveChanges();
            //UserInbox.SendNewsLetterWithVoucher(chosenUser, ctx, rewardIssue);
        }
        public static void GenerateNewsletterAboutATopic(HotelContext ctx, User user, string topic, string content)
        {
            var chosenUser = ctx.User.Where(u => u.Id == user.Id).First();
            var newsletter = new SystemMessage
            {
                Topic = topic,
                Content = content,
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First(),
            };

            ctx.Add(newsletter);
            ctx.SaveChanges();

            //UserInbox.SendNewsletterSystemMessage(user, ctx, newsletter);

        }
    }
}
