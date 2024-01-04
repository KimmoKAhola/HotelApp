using Autofac;

namespace TheSuiteSpot.Interfaces
{
    public interface IApplication
    {
        void Run(IContainer container);
    }
}