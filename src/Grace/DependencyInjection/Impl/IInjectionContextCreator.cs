namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// interface to create new injection context
    /// </summary>
    public interface IInjectionContextCreator
    {
        /// <summary>
        /// Create new injection context
        /// </summary>
        /// <param name="extraData">current extra data</param>
        /// <returns>new context</returns>
        IInjectionContext CreateContext(object extraData);
    }
}
