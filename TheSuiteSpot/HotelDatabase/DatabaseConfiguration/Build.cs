using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace TheSuiteSpot.HotelDatabase.DatabaseConfiguration
{
    public static class Build
    {
        public static DbContextOptionsBuilder<HotelContext> Configure()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<HotelContext>();

            options.UseSqlServer(connectionString);

            return options;
        }
        public static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true);

            var config = builder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            return connectionString;
        }
    }
}
