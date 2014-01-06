using System.Diagnostics;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Export condition that limits the export to only be used when there is no debugger
	/// </summary>
	public class WhenDebuggerIsNotAttached : IExportCondition
	{
		/// <summary>
		/// Returns true only when there is no debugger attached
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public bool ConditionMeet(IInjectionScope scope, IInjectionContext injectionContext, IExportStrategy exportStrategy)
		{
			return !Debugger.IsAttached;
		}
	}
}