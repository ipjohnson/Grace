using System.Diagnostics;

namespace Grace.DependencyInjection.Conditions
{
	/// <summary>
	/// Export condition that limits the export to only be used when the debugger is attached
	/// </summary>
	public class WhenDebuggerIsAttached : IExportCondition
	{
		/// <summary>
		/// Returns true when the debugger is attached
		/// </summary>
		/// <param name="scope"></param>
		/// <param name="injectionContext"></param>
		/// <param name="exportStrategy"></param>
		/// <returns></returns>
		public bool ConditionMeet(IInjectionScope scope, IInjectionContext injectionContext, IExportStrategy exportStrategy)
		{
			return Debugger.IsAttached;
		}
	}
}