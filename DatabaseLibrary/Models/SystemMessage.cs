using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
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
        public UserInbox UserInbox { get; set; } = null!;
    }
}
