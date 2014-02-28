using System;
using log4net;

namespace Grace.log4net
{
	/// <summary>
	/// Log wrapper around an ILog interface from log4net
	/// </summary>
	public class LogWrapper : Grace.Logging.ILog
	{
		private readonly ILog log;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="log">take ILog to wrap</param>
		public LogWrapper(ILog log)
		{
			this.log = log;
		}

		/// <summary>
		/// Is Debug Log Level Enabled
		/// </summary>
		public bool IsDebugEnabled
		{
			get { return log.IsDebugEnabled; }
		}

		/// <summary>
		/// Is Info Log Level  Enabled
		/// </summary>
		public bool IsInfoEnabled
		{
			get { return log.IsInfoEnabled; }
		}

		/// <summary>
		/// Is Warn Log Level Enabled
		/// </summary>
		public bool IsWarnEnabled
		{
			get { return log.IsWarnEnabled; }
		}

		/// <summary>
		/// Is Error Log Level Enabled
		/// </summary>
		public bool IsErrorEnabled
		{
			get { return log.IsErrorEnabled; }
		}

		/// <summary>
		/// Is Fatal Log Level Enabled
		/// </summary>
		public bool IsFatalEnabled
		{
			get { return log.IsFatalEnabled; }
		}

		/// <summary>
		/// Log Debug message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Debug(object message, Exception exp = null)
		{
			log.Debug(message, exp);
		}

		/// <summary>
		/// Log a formatted debug message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void DebugFormat(string format, params object[] formatParameters)
		{
			log.DebugFormat(format, formatParameters);
		}

		/// <summary>
		/// Log Info message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Info(object message, Exception exp = null)
		{
			log.Info(message, exp);
		}

		/// <summary>
		/// Log a formatted Info message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void InfoFormat(string format, params object[] formatParameters)
		{
			log.InfoFormat(format, formatParameters);
		}

		/// <summary>
		/// Log Warning message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Warn(object message, Exception exp = null)
		{
			log.Warn(message, exp);
		}

		/// <summary>
		/// Log a formatted Warning message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void WarnFormat(string format, params object[] formatParameters)
		{
			log.WarnFormat(format, formatParameters);
		}

		/// <summary>
		/// Log Error message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Error(object message, Exception exp = null)
		{
			log.Error(message, exp);
		}

		/// <summary>
		/// Log a formatted Error message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void ErrorFormat(string format, params object[] formatParameters)
		{
			log.ErrorFormat(format, formatParameters);
		}

		/// <summary>
		/// Log Fatal message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Fatal(object message, Exception exp = null)
		{
			log.Fatal(message, exp);
		}

		/// <summary>
		/// Log a formatted Fatal message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void FatalFormat(string format, params object[] formatParameters)
		{
			log.FatalFormat(format, formatParameters);
		}
	}
}