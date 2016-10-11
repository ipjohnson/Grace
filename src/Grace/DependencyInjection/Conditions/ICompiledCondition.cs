namespace Grace.DependencyInjection.Conditions
{
    public interface ICompiledCondition
    {
        bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext);
    }
}
