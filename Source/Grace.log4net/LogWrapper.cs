using System;
using log4net;

namespace Grace.log4net
{
	public class LogWrapper : Grace.Logging.ILog
	{
		private readonly ILog log;

		public LogWrapper(ILog log)
		{
			this.log = log;
		}

		public bool IsDebugEnabled
		{
			get { return log.IsDebugEnabled; }
		}

		public bool IsInfoEnabled
		{
			get { return log.IsInfoEnabled; }
		}

		public bool IsWarnEnabled
		{
			get { return log.IsWarnEnabled; }
		}

		public bool IsErrorEnabled
		{
			get { return log.IsErrorEnabled; }
		}

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