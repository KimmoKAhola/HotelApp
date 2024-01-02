using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;
using TheSuiteSpot.HotelDatabase.Models;
using TheSuiteSpot.Interfaces;

namespace TheSuiteSpot.HotelDatabase.DatabaseSeeding
{
    public class SystemMessageTypeSeeding(HotelContext dbContext) : IDataSeeding
    {
        public HotelContext DbContext { get; set; } = dbContext;

        public void SeedData()
        {
            if (!DbContext.MessageType.Any())
            {
                var type = new SystemMessageType
                {
                    Name = SystemMessageTypes.Subscription.ToString(),
                };

                var type2 = new SystemMessageType
                {
                    Name = SystemMessageTypes.System.ToString(),
                };

                var type3 = new SystemMessageType
                {
                    Name = SystemMessageTypes.Reward.ToString(),
                };

                var type4 = new SystemMessageType
                {
                    Name = SystemMessageTypes.UserToUser.ToString(),
                };

                var type5 = new SystemMessageType
                {
                    Name = SystemMessageTypes.Other.ToString(),
                };

                DbContext.Add(type);
                DbContext.Add(type2);
                DbContext.Add(type3);
                DbContext.Add(type4);
                DbContext.Add(type5);
                DbContext.SaveChanges();
            }
        }
    }
}
