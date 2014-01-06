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
		/// <param name="type"></param>
		/// <returns></returns>
		ILog GetLogger(Type type);

		/// <summary>
		/// Get a log instance by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ILog GetLogger(string name);
	}
}