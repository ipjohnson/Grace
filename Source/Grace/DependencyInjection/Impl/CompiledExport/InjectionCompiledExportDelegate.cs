namespace Grace.DependencyInjection.Impl.CompiledExport
{
	public class InjectionCompiledExportDelegate : FuncCompiledExportDelegate
	{
		/// <summary>
		/// Extra Data Key for Injection Target
		/// </summary>
		public const string InjectionTargetExtraDataKey = "InjectionTargetKey";

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="exportDelegateInfo"></param>
		/// <param name="owningScope"></param>
		public InjectionCompiledExportDelegate(CompiledExportDelegateInfo exportDelegateInfo, IInjectionScope owningScope)
			: base(exportDelegateInfo, LocateInjectionObject, owningScope)
		{
		}

		/// <summary>
		/// Returns the injection target
		/// </summary>
		/// <param name="injectionScope"></param>
		/// <param name="injectionContext"></param>
		/// <returns></returns>
		public static object LocateInjectionObject(IInjectionScope injectionScope, IInjectionContext injectionContext)
		{
			return injectionContext.GetExtraData(InjectionTargetExtraDataKey);
		}
	}
}