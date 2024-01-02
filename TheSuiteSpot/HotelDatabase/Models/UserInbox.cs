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
