namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Creates new injection contexts
    /// </summary>
    public class InjectionContextCreator : IInjectionContextCreator
    {
        /// <summary>
        /// Create new injection context
        /// </summary>
        /// <param name="extraData">current extra data</param>
        /// <returns>new context</returns>
        public IInjectionContext CreateContext(object extraData)
        {
            return extraData as IInjectionContext ?? new InjectionContext(extraData);
        }
    }
}
