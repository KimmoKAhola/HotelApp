using Autofac;
using TheSuiteSpot.HotelDatabase.DatabaseConfiguration;

namespace TheSuiteSpot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var container = Container.Configure();
            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<Application>();
                app.Run(container);
            }
        }
    }
}
