using System;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Defined what environement the application is running in or compenent is Exported in
	/// </summary>
	[Flags]
	public enum ExportEnvironment
	{
		/// <summary>
		/// Can be used in any environment
		/// </summary>
		Any = 1,

		/// <summary>
		/// Best used at runtime
		/// </summary>
		RunTime = 2,

		/// <summary>
		/// Can only be used at runtime
		/// </summary>
		RunTimeOnly = 4,

		/// <summary>
		/// Best used at design time
		/// </summary>
		DesignTime = 8,

		/// <summary>
		/// Only used at design time
		/// </summary>
		DesignTimeOnly = 16,

		/// <summary>
		/// Best used at unit test time
		/// </summary>
		UnitTest = 32,

		/// <summary>
		/// Only used at unit test time
		/// </summary>
		UnitTestOnly = 64,
	}
}