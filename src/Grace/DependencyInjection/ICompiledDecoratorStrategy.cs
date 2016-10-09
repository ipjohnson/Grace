namespace Grace.DependencyInjection
{
    public interface ICompiledDecoratorStrategy : IConfigurableActivationStrategy, IDecoratorOrExportActivationStrategy
    {
        /// <summary>
        /// Apply the decorator after a lifestyle has been used
        /// </summary>
        bool ApplyAfterLifestyle { get; set; }
    }
}
