using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.Logging;
using NL = NLog;

namespace Grace.NLog
{
	/// <summary>
	/// Log service that creates NLog loggers
	/// </summary>
	public class LogService : ILogService
	{
		/// <summary>
		/// Get a logger by type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ILog GetLogger(Type type)
		{
			return new LogWrapper(NL.LogManager.GetLogger(type.FullName));
		}

		/// <summary>
		/// Get Logger by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ILog GetLogger(string name)
		{
			return new LogWrapper(NL.LogManager.GetLogger(name));
		}
	}
}
