namespace Grace.Tests.Classes.Simple
{
    public interface IConventionBasedService
    {
        IMultipleService ServiceA { get; }

        IMultipleService ServiceB { get; }

    }

    public class ConventionBasedService : IConventionBasedService
    {
        public ConventionBasedService(IMultipleService serviceA, IMultipleService serviceB)
        {
            ServiceA = serviceA;
            ServiceB = serviceB;
        }

        public IMultipleService ServiceA { get; }

        public IMultipleService ServiceB { get; }
    }
}
