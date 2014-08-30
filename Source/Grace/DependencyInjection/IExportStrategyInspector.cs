using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes the implement this interface can be used 
	/// </summary>
	public interface IExportStrategyInspector
	{
		/// <summary>
		/// Inspect a strategy as it's being added to the container.
		/// It will be called before the strategy Initialize method is called
		/// </summary>
		/// <param name="exportStrategy"></param>
		void Inspect([NotNull]IExportStrategy exportStrategy);
	}
}