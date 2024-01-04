using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;

namespace TheSuiteSpot.Interfaces
{
    public interface IMenu : IDbContext
    {
        void Display();
        public string MenuName { get; set; }
        public List<IMenu> Menus { get; set; }
        void PrintOptions();
        void ReturnToMainMenu();
    }
}
