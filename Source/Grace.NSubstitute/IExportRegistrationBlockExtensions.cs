using Grace.DependencyInjection;

namespace Grace.NSubstitute
{
	/// <summary>
	/// C# extension classes for NSubstitute
	/// </summary>
   // ReSharper disable once InconsistentNaming
	public static class IExportRegistrationBlockExtensions
	{
		/// <summary>
		/// Exports an interface using NSubstitute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="block"></param>
		/// <returns></returns>
		public static ISubstituteExportStrategyConfiguration<T> Substitute<T>(this IExportRegistrationBlock block)
			where T : class
		{
			NSubstituteExportStrategy<T> strategy = new NSubstituteExportStrategy<T>();

			block.AddExportStrategy(strategy);

			return new SubstituteExportStrategyConfiguration<T>(strategy);
		}
	}
}