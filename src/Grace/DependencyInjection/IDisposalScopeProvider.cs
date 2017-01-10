namespace Grace.DependencyInjection
{
    /// <summary>
    /// Provides IDisposalScope for use during locate
    /// </summary>
    public interface IDisposalScopeProvider
    {
        /// <summary>
        /// Provide a disposal scope for locator
        /// </summary>
        /// <param name="locatorScope">locator scope</param>
        /// <returns>new disposal scope</returns>
        IDisposalScope ProvideDisposalScope(IExportLocatorScope locatorScope);
    }
}
