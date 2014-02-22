using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes that implement this interface can be used to provide an import value during construction
	/// 
	/// </summary>
	public interface IExportValueProvider
	{
		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		object Activate([NotNull] IInjectionScope exportInjectionScope,
			[NotNull] IInjectionContext context,
			[CanBeNull] ExportStrategyFilter consider);
	}
}