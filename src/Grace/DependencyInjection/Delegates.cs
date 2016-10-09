namespace Grace.DependencyInjection
{

    public delegate object ActivationStrategyDelegate(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext injectionContext);

    /// <summary>
    /// USed to filter out exports at container configuration time
    /// </summary>
    /// <param name="staticContext"></param>
    /// <param name="strategy"></param>
    /// <returns></returns>
    public delegate bool ExportStrategyStaticFilter(StaticInjectionContext staticContext, ICompiledExportStrategy strategy);

    /// <summary>
    /// Used to filter out exports
    /// </summary>
    /// <param name="strategy">strategy to filter</param>
    /// <returns>return true if the strategy should be used</returns>
    public delegate bool ExportStrategyFilter(ICompiledExportStrategy strategy);
    
    /// <summary>
    /// This delegate will locate an export using a Key
    /// </summary>
    /// <typeparam name="TKey">key type to locate with</typeparam>
    /// <typeparam name="TValue">value type to locate</typeparam>
    /// <param name="key">key to locate with</param>
    /// <returns>located value</returns>
    public delegate TValue KeyedLocateDelegate<in TKey, out TValue>(TKey key);
}
