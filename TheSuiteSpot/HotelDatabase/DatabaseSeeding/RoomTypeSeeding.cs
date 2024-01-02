using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    public class RoomTypeSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;
        public void SeedData()
        {
            if (!DbContext.RoomType.Any())
            {
                List<IEntity> entities = new List<IEntity>();
                var juniorSuite = new RoomType
                {
                    SuiteName = $"{Enum.GetName(SuiteTypes.Junior)} Suite",
                    IsDoubleRoom = false,
                    NumberOfExtraBeds = 0,
                };
                entities.Add(juniorSuite);

                var executiveSuite = new RoomType
                {
                    SuiteName = $"{Enum.GetName(SuiteTypes.Executive)} Suite",
                    IsDoubleRoom = true,
                    NumberOfExtraBeds = 0
                };
                entities.Add(executiveSuite);
                var deluxeSuite = new RoomType
                {
                    SuiteName = $"{Enum.GetName(SuiteTypes.Deluxe)} Suite",
                    IsDoubleRoom = true,
                    NumberOfExtraBeds = 1
                };
                entities.Add(deluxeSuite);

                var royalSuite = new RoomType
                {
                    SuiteName = $"{Enum.GetName(SuiteTypes.Royal)} Suite",
                    IsDoubleRoom = true,
                    NumberOfExtraBeds = 2
                };
                entities.Add(royalSuite);

                foreach (var item in entities)
                {
                    DbContext.RoomType.Add((RoomType)item);
                }

                DbContext.SaveChanges();
            }
        }
    }
}