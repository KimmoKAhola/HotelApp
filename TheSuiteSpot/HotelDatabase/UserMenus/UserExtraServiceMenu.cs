using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.UserMenus
{
    public class UserExtraServiceMenu(HotelContext dbContext) : IUserMenu
    {
        public string MenuName { get; set; } = "Extra services";
        public List<IUserMenu> Menus { get; set; }
        public HotelContext DbContext { get; set; } = dbContext;

        public void Display()
        {
            throw new NotImplementedException();
        }

        public void PrintOptions()
        {
            throw new NotImplementedException();
        }

        public void ReturnToMainMenu()
        {
            throw new NotImplementedException();
        }
    }
}
