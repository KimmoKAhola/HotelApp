﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class Initialize(IEnumerable<IDataSeeding> dataSeeding, HotelContext dbContext)
    {
        public IEnumerable<IDataSeeding> DataSeedings { get; set; } = dataSeeding;
        public HotelContext DbContext { get; set; } = dbContext;

        public void Seed()
        {
            if (!(DbContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                DbContext.Database.Migrate();

                foreach (var seeding in dataSeeding)
                {
                    seeding.SeedData();
                }
                DbContext.SaveChanges();
            }
            else
            {
                foreach (var seeding in dataSeeding)
                {
                    if (seeding is ReviewSeeding)
                    {
                        seeding.SeedData();
                    }
                }
            }
        }
    }
}
