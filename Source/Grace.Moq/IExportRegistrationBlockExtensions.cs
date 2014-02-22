using Grace.DependencyInjection;

namespace Grace.Moq
{
	/// <summary>
	/// Contains C# extensions for creating Moq objects
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// Creates a Moq export for a particular type
		/// </summary>
		/// <typeparam name="T">type to mock</typeparam>
		/// <param name="block">registration block</param>
		/// <returns></returns>
		public static IMoqExportStrategyConfiguration<T> Moq<T>(this IExportRegistrationBlock block) where T : class
		{
			MoqExportStrategy<T> exportStrategy = new MoqExportStrategy<T>();

			MoqExportStrategyConfiguration<T> returnValue =
				new MoqExportStrategyConfiguration<T>(exportStrategy);

			block.AddExportStrategy(exportStrategy);

			return returnValue;
		}
	}
}