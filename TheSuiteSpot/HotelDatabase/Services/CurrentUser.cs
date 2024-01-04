using Autofac.Core.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.Models;

namespace TheSuiteSpot.HotelDatabase.Services
{
    public class CurrentUser
    {
        private CurrentUser()
        {

        }
        private static CurrentUser? _instance = null;
        public User User { get; private set; }

        public static CurrentUser Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void SetCurrentUser(User user)
        {
            _instance = new CurrentUser();
            _instance.User = user;
        }
    }
}
