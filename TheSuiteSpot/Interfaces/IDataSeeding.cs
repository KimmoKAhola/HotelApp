using Microsoft.EntityFrameworkCore;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;

namespace TheSuiteSpot.Interfaces
{
    public interface IDataSeeding : IDbContext
    {
        void SeedData();
    }
}