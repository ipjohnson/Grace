using System;

namespace Grace.Logging
{
	/// <summary>
	/// Log service that writes everything to the debug console
	/// </summary>
	public class DebugConsoleLogService : ILogService
	{
		/// <summary>
		/// Get a log instance based on type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ILog GetLogger(Type type)
		{
			return new DebugConsoleLog(type.FullName);
		}

		/// <summary>
		/// Get a log instance by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ILog GetLogger(string name)
		{
			return new DebugConsoleLog(name);
		}
	}
}