using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;

namespace TheSuiteSpot.Interfaces
{
    public interface IDbContext
    {
        public HotelContext DbContext { get; set; }
    }
}
