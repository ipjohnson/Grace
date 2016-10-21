namespace Grace.DependencyInjection
{
    public interface IDisposalScopeProvider
    {
        IDisposalScope ProvideDisposalScope(IExportLocatorScope locatorScope);
    }
}
