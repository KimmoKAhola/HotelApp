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
    public enum SystemMessageTypes
    {
        System,
        Subscription,
        Reward,
        UserToUser,
        Other
    }
    public class SystemMessageType : IEntity
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        //public List<SystemMessage>? SystemMessage { get; set; }
    }
}
