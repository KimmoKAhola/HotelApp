using Autofac;

namespace TheSuiteSpot
{
    public interface IApplication
    {
        void Run(IContainer container);
    }
}