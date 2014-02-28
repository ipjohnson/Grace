using System;

namespace Grace.Logging
{
	/// <summary>
	/// Log service to be used by Logger
	/// </summary>
	public interface ILogService
	{
		/// <summary>
		/// Get a log instance based on type
		/// </summary>
		/// <param name="type">type of logger to get</param>
		/// <returns>ILog instance</returns>
		ILog GetLogger(Type type);

		/// <summary>
		/// Get a log instance by name
		/// </summary>
		/// <param name="name">logger name to get</param>
		/// <returns>ILog instance</returns>
		ILog GetLogger(string name);
	}
}