namespace Grace.DependencyInjection
{
    /// <summary>
    /// Delegate for activating a strategy
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="disposalScope"></param>
    /// <param name="injectionContext"></param>
    /// <returns></returns>
    public delegate object ActivationStrategyDelegate(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext injectionContext);

    /// <summary>
    /// Delegate for activating a strongly typed strategy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="scope"></param>
    /// <param name="disposalScope"></param>
    /// <param name="injectionContext"></param>
    /// <returns></returns>
    public delegate T TypedActivationStrategyDelegate<out T>(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext injectionContext);

    /// <summary>
    /// Delegate for injecting value
    /// </summary>
    /// <param name="scope">injection scope</param>
    /// <param name="disposalScope">disposal scope</param>
    /// <param name="injectionContext">injection context</param>
    /// <param name="injectedInstance">instance to inject</param>
    public delegate void InjectionStrategyDelegate(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext injectionContext, object injectedInstance);
    
    /// <summary>
    /// Used to filter out exports at container configuration time
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
    public delegate bool ActivationStrategyFilter(IActivationStrategy strategy);
    
    /// <summary>
    /// This delegate will locate an export using a Key
    /// </summary>
    /// <typeparam name="TKey">key type to locate with</typeparam>
    /// <typeparam name="TValue">value type to locate</typeparam>
    /// <param name="key">key to locate with</param>
    /// <returns>located value</returns>
    public delegate TValue KeyedLocateDelegate<in TKey, out TValue>(TKey key);
}
