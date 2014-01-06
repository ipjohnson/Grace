using System;

namespace Grace.Logging
{
	/// <summary>
	/// Returns loggers that do nothing
	/// </summary>
	public class DevNullLogService : ILogService
	{
		private readonly DevNullLog log = new DevNullLog();

		/// <summary>
		/// Get a log instance based on type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ILog GetLogger(Type type)
		{
			return log;
		}

		/// <summary>
		/// Get a log instance by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ILog GetLogger(string name)
		{
			return log;
		}
	}
}