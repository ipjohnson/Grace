using System;

namespace Grace.Logging
{
	/// <summary>
	/// Static logging class for the framework. 
	/// </summary>
	public static class Logger
	{
		private static volatile ILogService logServiceInstance;

		/// <summary>
		/// Get the log service for the framework
		/// </summary>
		public static ILogService LogService
		{
			get
			{
				if (logServiceInstance == null)
				{
					logServiceInstance = new DevNullLogService();
				}

				return logServiceInstance;
			}
		}

		/// <summary>
		/// Set the logger to be used for the containers
		/// </summary>
		/// <param name="logService"></param>
		public static void SetLogService(ILogService logService)
		{
			logServiceInstance = logService;
		}

		/// <summary>
		/// Get an ILog instance
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ILog GetLogger(Type type)
		{
			return LogService.GetLogger(type);
		}

		/// <summary>
		/// Get an ILog instance
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static ILog GetLogger<T>()
		{
			return LogService.GetLogger(typeof(T));
		}

		/// <summary>
		/// Get an ILog by name
		/// </summary>
		/// <param name="logName"></param>
		/// <returns></returns>
		public static ILog GetLogger(string logName)
		{
			return LogService.GetLogger(logName);
		}

		/// <summary>
		/// Log a debug message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="supplemental"></param>
		/// <param name="exp"></param>
		public static void Debug(string message, string supplemental = null, Exception exp = null)
		{
			LogService.GetLogger(supplemental).Debug(message, exp);
		}

		/// <summary>
		/// Log an error message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="supplemental"></param>
		/// <param name="exp"></param>
		public static void Error(string message, string supplemental = null, Exception exp = null)
		{
			LogService.GetLogger(supplemental).Debug(message, exp);
		}
	}
}