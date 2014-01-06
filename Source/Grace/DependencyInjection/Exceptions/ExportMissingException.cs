using System;

namespace Grace.DependencyInjection.Exceptions
{
	public class ExportMissingException : Exception
	{
		public ExportMissingException(string exportName) :
			base("Could not locate export for " + exportName)
		{
		}
	}
}