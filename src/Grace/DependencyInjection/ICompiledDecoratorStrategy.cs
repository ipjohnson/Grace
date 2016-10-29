namespace Grace.DependencyInjection
{
    /// <summary>
    /// Compiled decorator strategy
    /// </summary>
    public interface ICompiledDecoratorStrategy : IConfigurableActivationStrategy, IDecoratorOrExportActivationStrategy
    {
        /// <summary>
        /// Apply the decorator after a lifestyle has been used
        /// </summary>
        bool ApplyAfterLifestyle { get; set; }
    }
}
