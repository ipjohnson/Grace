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
		/// <param name="exportInjectionScope">injection scope</param>
		/// <param name="context">injection context</param>
		/// <param name="consider">export filter</param>
		/// <param name="locateKey"></param>
		/// <returns>activated object</returns>
		object Activate([NotNull] IInjectionScope exportInjectionScope,
							 [NotNull] IInjectionContext context,
							 [CanBeNull] ExportStrategyFilter consider,
							 [CanBeNull] object locateKey);
	}
}