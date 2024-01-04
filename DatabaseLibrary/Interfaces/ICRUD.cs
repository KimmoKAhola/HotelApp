using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;

namespace TheSuiteSpot.Interfaces
{
    public interface ICRUD : IDbContext
    {
        void Create(HotelContext ctx);
        void ExactSearch(HotelContext ctx);
        void ReadAll(HotelContext ctx);
        void Update(HotelContext ctx);
        void SoftDelete(HotelContext ctx);
        void GeneralSearch(HotelContext ctx);
    }
}
