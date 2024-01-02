using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSuiteSpot.Interfaces
{
    public interface IUserMenu : IDbContext
    {
        void Display();
        public string MenuName { get; set; }
        public List<IUserMenu> Menus { get; set; }
        void PrintOptions();
        void ReturnToMainMenu();
    }
}
