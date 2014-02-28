using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Logging;
using CommonLogging = Common.Logging;


namespace Grace.Common.Logging
{
	/// <summary>
	/// Log wrapper around Common.Logging.LogManager
	/// </summary>
	public class LogService : Grace.Logging.ILogService
	{
		/// <summary>
		/// Get a log instance based on type
		/// </summary>
		/// <param name="type">type of logger to get</param>
		/// <returns>
		/// ILog instance
		/// </returns>
		public ILog GetLogger(Type type)
		{
			var logger = CommonLogging.LogManager.GetLogger(type);

			return new LogWrapper(logger);
		}

		/// <summary>
		/// Get a log instance by name
		/// </summary>
		/// <param name="name">logger name to get</param>
		/// <returns>
		/// ILog instance
		/// </returns>
		public ILog GetLogger(string name)
		{
			var logger = CommonLogging.LogManager.GetLogger(name);

			return new LogWrapper(logger);
		}
	}
}
