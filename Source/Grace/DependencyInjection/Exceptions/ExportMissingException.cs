using System;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// Exception thrown when an export can't be found
	/// </summary>
	public class ExportMissingException : Exception
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		/// <param name="exportName">export name</param>
		public ExportMissingException(string exportName) :
			base("Could not locate export for " + exportName)
		{
		}
	}
}