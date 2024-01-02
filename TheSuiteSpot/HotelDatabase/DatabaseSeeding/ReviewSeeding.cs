using Bogus;
using Microsoft.EntityFrameworkCore;
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
    public class ReviewSeeding(HotelContext dbContext) : IDataSeeding
    {
        private readonly int _numberOfSeededReviews = 500;
        private readonly int _maxRating = 100;
        private readonly int _minRating = 65; // boost our numbers. Business rule
        private readonly int _cutoff = (100 + 65) / 2;

        public HotelContext DbContext { get; set; } = dbContext;
        public void SeedData()
        {
            var faker = new Faker();
            var random = new Random();
            var roomsToReview = DbContext.Room.Include(rt => rt.RoomType).ToList();
            var availableUsers = DbContext.User.ToList();
            for (int i = 0; i < _numberOfSeededReviews; i++)
            {
                var minRating = _minRating;
                var randomRoomIndex = random.Next(0, roomsToReview.Count);
                if (_numberOfSeededReviews % 2 == 0)
                {
                    if (roomsToReview[randomRoomIndex].RoomType.SuiteName.Contains("Royal"))
                    {
                        minRating += 25;
                    }
                    else if (roomsToReview[randomRoomIndex].RoomType.SuiteName.Contains("Deluxe"))
                    {
                        minRating += 10;
                    }
                    else if (roomsToReview[randomRoomIndex].RoomType.SuiteName.Contains("Executive"))
                    {
                        minRating += 5;
                    }
                    var review = new Review
                    {
                        ReviewText = faker.Rant.Review("room"),
                        StarsGiven = (byte)faker.Random.Int(minRating, _maxRating),
                        Room = roomsToReview[randomRoomIndex],
                    };
                    if (review.StarsGiven > _cutoff)
                        review.UserName = faker.Internet.UserName();
                    DbContext.Add(review);
                }
                else
                {
                    var randomIndex = random.Next(1, availableUsers.Count);
                    var user = availableUsers[randomIndex];
                    var review = new Review
                    {
                        ReviewText = faker.Rant.Review("room"),
                        StarsGiven = (byte)faker.Random.Int(_minRating, _maxRating),
                        Room = roomsToReview[randomRoomIndex],
                    };
                    if (review.StarsGiven > _cutoff)
                        review.UserName = user.UserName;
                    DbContext.Add(review);
                }
            }
            DbContext.SaveChanges();
        }
    }
}
