using System;
using Grace.Logging;
using l4n = log4net;

namespace Grace.log4net
{
	public class LogService : ILogService
	{
		public ILog GetLogger(Type type)
		{
			l4n.ILog log = l4n.LogManager.GetLogger(type);

			return new LogWrapper(log);
		}

		public ILog GetLogger(string name)
		{
			l4n.ILog log = l4n.LogManager.GetLogger(name);

			return new LogWrapper(log);
		}
	}
}