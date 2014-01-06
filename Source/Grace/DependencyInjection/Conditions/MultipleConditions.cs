using System;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Combines multiple conditions into one
	/// </summary>
	public class MultipleConditions : IExportCondition
	{
		private readonly IExportCondition[] exportConditions;

		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="exportConditions"></param>
		public MultipleConditions(params IExportCondition[] exportConditions)
		{
			if (exportConditions == null)
			{
				throw new ArgumentNullException("exportConditions");
			}

			this.exportConditions = exportConditions;
		}

		/// <summary>
		/// Called to determine if the export strategy meets the condition to be activated
		/// </summary>
		/// <param name="scope">injection scope that this export exists in</param>
		/// <param name="injectionContext">injection context for this request</param>
		/// <param name="exportStrategy">export strategy being tested</param>
		/// <returns>true if the export meets the condition</returns>
		public bool ConditionMeet(IInjectionScope scope, IInjectionContext injectionContext, IExportStrategy exportStrategy)
		{
			foreach (IExportCondition exportCondition in exportConditions)
			{
				if (!exportCondition.ConditionMeet(scope, injectionContext, exportStrategy))
				{
					return false;
				}
			}

			return true;
		}
	}
}