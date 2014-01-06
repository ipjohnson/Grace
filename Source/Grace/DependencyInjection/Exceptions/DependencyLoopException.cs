using System;

namespace Grace.DependencyInjection.Exceptions
{
	/// <summary>
	/// This exception is thrown when a loop is detected trying to resolve an export.
	/// </summary>
	public class DependencyLoopException : Exception
	{
	}
}