namespace Grace.DependencyInjection
{
	/// <summary>
	/// Delegate representing scope in wich this export is being activated
	/// </summary>
	/// <param name="injectionScope">scope that the export startegy is attached to, this can be different than it's owning scope</param>
	/// <param name="context">context for the injection</param>
	/// <returns>new activated object</returns>
	public delegate object ExportActivationDelegate(
		IInjectionScope injectionScope,
		IInjectionContext context);

	/// <summary>
	/// Used to filter out exports
	/// </summary>
	/// <param name="context">context to use during filtering</param>
	/// <param name="strategy">strategy to filter</param>
	/// <returns>return true if the strategy should be used</returns>
	public delegate bool ExportStrategyFilter(IInjectionContext context, IExportStrategy strategy);

	/// <summary>
	/// Used to compare two exports within a particular export environment
	/// </summary>
	/// <param name="x">x</param>
	/// <param name="y">y</param>
	/// <param name="exportEnvironment">current environment</param>
	/// <returns></returns>
	public delegate int ExportStrategyComparer(IExportStrategy x, IExportStrategy y, ExportEnvironment exportEnvironment);

	/// <summary>
	/// This delegate allows you to provide extra registration during scope creation
	/// </summary>
	/// <param name="registration">registration object</param>
	public delegate void ExportRegistrationDelegate(IExportRegistrationBlock registration);

	/// <summary>
	/// This delegate can be used to provide an export
	/// </summary>
	/// <typeparam name="T">type being returned</typeparam>
	/// <param name="injectionScope">injection scope</param>
	/// <param name="injectionContext">injection context</param>
	/// <returns></returns>
	public delegate T ExportFunction<out T>(
		IInjectionScope injectionScope,
		IInjectionContext injectionContext);

	/// <summary>
	/// Using this delegate you can provide custom logic to the activation process overriding 
	/// </summary>
	/// <param name="scope">injection scope for this export</param>
	/// <param name="injectionContext">injection context for this call</param>
	/// <param name="injectedObject">injected object</param>
	/// <returns>return the initial object or return a wrapping object</returns>
	public delegate object EnrichWithDelegate(
		IInjectionScope scope,
		IInjectionContext injectionContext,
		object injectedObject);

	/// <summary>
	/// This delegate allows the developer to perform some cleanup before Dispose is called
	/// </summary>
	/// <param name="objectBeingDisposed"></param>
	public delegate void BeforeDisposalCleanupDelegate(object objectBeingDisposed);

	/// <summary>
	/// Delegate is used to figure out if the export strategy meets the proper condition
	/// </summary>
	/// <param name="scope">scope the export is in</param>
	/// <param name="injectionContext">injection context for this call</param>
	/// <param name="exportStrategy">export strategy being considered</param>
	/// <returns>return true if the export is to be considered</returns>
	public delegate bool ExportConditionDelegate(
		IInjectionScope scope,
		IInjectionContext injectionContext,
		IExportStrategy exportStrategy);


	/// <summary>
	/// This delegate will locate an export using a Key
	/// </summary>
	/// <typeparam name="TKey">key type to locate with</typeparam>
	/// <typeparam name="TValue">value type to locate</typeparam>
	/// <param name="key">key to locate with</param>
	/// <returns>located value</returns>
	public delegate TValue KeyedLocateDelegate<in TKey, out TValue>(TKey key);
}