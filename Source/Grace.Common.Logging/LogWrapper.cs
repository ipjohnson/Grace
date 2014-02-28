using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.Common.Logging
{
	/// <summary>
	/// Wraps the ILog interface from Common.Logging
	/// </summary>
	public class LogWrapper : Grace.Logging.ILog
	{
		private global::Common.Logging.ILog log;

		/// <summary>
		/// default constructor
		/// </summary>
		/// <param name="log">log instance to wrap</param>
		public LogWrapper(global::Common.Logging.ILog log)
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
			get { return log.IsDebugEnabled; }
		}

		/// <summary>
		/// Is Warn Log Level Enabled
		/// </summary>
		public bool IsWarnEnabled
		{
			get { return log.IsDebugEnabled; }
		}

		/// <summary>
		/// Is Error Log Level Enabled
		/// </summary>
		public bool IsErrorEnabled
		{
			get { return log.IsDebugEnabled; }
		}

		/// <summary>
		/// Is Fatal Log Level Enabled
		/// </summary>
		public bool IsFatalEnabled
		{
			get { return log.IsDebugEnabled; }
		}


		/// <summary>
		/// Log a Debug message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Debug(object message, Exception exp = null)
		{
			log.Debug(message, exp);
		}

		/// <summary>
		/// Log a Debug formatted message
		/// </summary>
		/// <param name="format">format object</param>
		/// <param name="formatParameters">format parameters</param>
		public void DebugFormat(string format, params object[] formatParameters)
		{
			log.DebugFormat(format, formatParameters);
		}

		/// <summary>
		/// Log a Info message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Info(object message, Exception exp = null)
		{
			log.Info(message, exp);
		}

		/// <summary>
		/// Log a Info formatted message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void InfoFormat(string format, params object[] formatParameters)
		{
			log.InfoFormat(format, formatParameters);
		}

		/// <summary>
		/// Log a Warn message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Warn(object message, Exception exp = null)
		{
			log.Warn(message, exp);
		}

		/// <summary>
		/// Log a Warn formatted message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void WarnFormat(string format, params object[] formatParameters)
		{
			log.WarnFormat(format, formatParameters);
		}

		/// <summary>
		/// Log an Error message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Error(object message, Exception exp = null)
		{
			log.Error(message, exp);
		}

		/// <summary>
		/// Log an Error format
		/// </summary>
		/// <param name="format">format message</param>
		/// <param name="formatParameters">format parameters</param>
		public void ErrorFormat(string format, params object[] formatParameters)
		{
			log.ErrorFormat(format, formatParameters);
		}

		/// <summary>
		/// Log an Fatal message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Fatal(object message, Exception exp = null)
		{
			log.Fatal(message, exp);
		}

		/// <summary>
		/// Log an Fatal format
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void FatalFormat(string format, params object[] formatParameters)
		{
			log.FatalFormat(format, formatParameters);
		}
	}
}
