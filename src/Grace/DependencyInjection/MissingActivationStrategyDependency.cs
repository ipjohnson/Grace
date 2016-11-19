namespace Grace.DependencyInjection
{
    /// <summary>
    /// Describes a missing dependency for an activation strategy
    /// </summary>
    public class MissingActivationStrategyDependency
    {
        public ActivationStrategyDependency Dependency { get; }
    }
}
