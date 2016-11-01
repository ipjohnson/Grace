namespace Grace.DependencyInjection.Impl.Wrappers
{
    /// <summary>
    /// Provide a list of compiled wrappers
    /// </summary>
    public interface IDefaultWrapperCollectionProvider
    {
        /// <summary>
        /// Provide container for wrappers
        /// </summary>
        /// <param name="scope">scope</param>
        /// <returns>wrapper collection container</returns>
        IActivationStrategyCollectionContainer<ICompiledWrapperStrategy> ProvideCollection(IInjectionScope scope);
    }
}
