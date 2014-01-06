namespace Grace.DependencyInjection
{
	/// <summary>
	/// Classes that implement this interface can be used to inspect strategies as they are being added to the container
	/// </summary>
	public interface IStrategyInspector
	{
		/// <summary>
		/// Called everytime a strategy about to be added to the the IInjectionScope
		/// </summary>
		/// <param name="exportStrategy"></param>
		void StrategyInitializing(IExportStrategy exportStrategy);
	}
}