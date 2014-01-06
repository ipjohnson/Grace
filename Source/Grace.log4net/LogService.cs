using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
