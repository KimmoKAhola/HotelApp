using System.ComponentModel.DataAnnotations;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.Models
{
    public class SystemMessage : IEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Topic { get; set; } = null!;

        [Required]
        public string Sender { get; set; } = null!;

        [MaxLength(5000)]
        public string Content { get; set; } = null!;
        public DateTime DateSent { get; set; } = DateTime.Now;
        public Voucher? Voucher { get; set; }

        public bool IsRead { get; set; } = false;

        public SystemMessageType MessageType { get; set; } = null!;
        public static void SendRewardMessage(HotelContext ctx, decimal discountPercentage, User receiver)
        {
            var randomString = Voucher.GenerateVoucherCode(discountPercentage);
            var voucher = new Voucher
            {
                VoucherCode = $"CODE-{randomString.VoucherCode}",
                DiscountPercentage = discountPercentage,
            };
            ctx.Add(voucher);
            ctx.SaveChanges();

            var chosenUser = ctx.User.Where(u => u.Id == receiver.Id).First();
            var reward = new SystemMessage
            {
                Topic = "A reward to our most valued customer.",
                Sender = ctx.User.Where(u => u.UserName == "System").First().UserName,
                Content = "Thank you, dear customer, for your patronage. As a reward, we will award you with a voucher.",
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.Reward.ToString()).First(),
            };
            reward.Voucher = voucher;
            receiver.UserInbox.Messages.Add(reward);
            ctx.SaveChanges();
            //UserInbox.SendMessageWithVoucher(chosenUser, ctx, reward);
        }
        public static void SendSystemMessage(HotelContext ctx, User receiver, string topic, string content)
        {
            var chosenUser = ctx.User.Where(u => u.Id == receiver.Id).First();
            var newsletter = new SystemMessage
            {
                Topic = topic,
                Sender = receiver.UserName,
                Content = content,
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.System.ToString()).First(),
            };

            receiver.UserInbox.Messages.Add(newsletter);
            ctx.SaveChanges();
        }

        public static void SendMessageBetweenUsers(HotelContext ctx, User sender, User receiver)
        {
            var message = new SystemMessage
            {
                Topic = "SEX ME",
                Sender = sender.UserName,
                Content = "U WANT SEX ME",
                MessageType = ctx.MessageType.Where(n => n.Name == SystemMessageTypes.UserToUser.ToString()).First(),
            };

            receiver.UserInbox.Messages.Add(message);
            ctx.SaveChanges();
        }
    }
}
