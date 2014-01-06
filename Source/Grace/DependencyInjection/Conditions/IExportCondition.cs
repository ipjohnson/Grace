namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// any class that implements this interface can be used as a condition on an export strategy
	/// </summary>
	public interface IExportCondition
	{
		/// <summary>
		/// Called to determine if the export strategy meets the condition to be activated
		/// </summary>
		/// <param name="scope">injection scope that this export exists in</param>
		/// <param name="injectionContext">injection context for this request</param>
		/// <param name="exportStrategy">export strategy being tested</param>
		/// <returns>true if the export meets the condition</returns>
		bool ConditionMeet(IInjectionScope scope,
			IInjectionContext injectionContext,
			IExportStrategy exportStrategy);
	}
}