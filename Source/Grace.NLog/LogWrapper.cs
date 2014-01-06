using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NL = NLog;

namespace Grace.NLog
{

	/// <summary>
	/// Wrapper class around NLog.Logger
	/// </summary>
	public class LogWrapper : Grace.Logging.ILog
	{
		private readonly NL.Logger logger;

		/// <summary>
		/// Default Constructor takes a logger
		/// </summary>
		/// <param name="logger"></param>
		public LogWrapper(NL.Logger logger)
		{
			this.logger = logger;
		}

		/// <summary>
		/// Is Debug Level Enabled
		/// </summary>
		public bool IsDebugEnabled
		{
			get { return logger.IsDebugEnabled; }
		}

		/// <summary>
		/// Is Info Level Enabled
		/// </summary>
		public bool IsInfoEnabled
		{
			get { return logger.IsInfoEnabled; }
		}

		/// <summary>
		/// Is Warning Level Enabled
		/// </summary>
		public bool IsWarnEnabled
		{
			get { return logger.IsWarnEnabled; }
		}

		/// <summary>
		/// Is Error Level Enabled
		/// </summary>
		public bool IsErrorEnabled
		{
			get { return logger.IsErrorEnabled; }
		}

		/// <summary>
		/// Is Fatal Level Enabled
		/// </summary>
		public bool IsFatalEnabled
		{
			get { return logger.IsFatalEnabled; }
		}

		/// <summary>
		/// Log Debug message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Debug(object message, Exception exp = null)
		{
			if (exp != null)
			{
				logger.DebugException(message.ToString(), exp);
			}
			else
			{
				logger.Debug(message);
			}
		}

		/// <summary>
		/// Log a formatted debug message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void DebugFormat(string format, params object[] formatParameters)
		{
			logger.Debug(format, formatParameters);
		}

		/// <summary>
		/// Log Info message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Info(object message, Exception exp = null)
		{
			if (exp != null)
			{
				logger.InfoException(message.ToString(), exp);
			}
			else
			{
				logger.Info(message);
			}
		}

		/// <summary>
		/// Log a formatted Info message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void InfoFormat(string format, params object[] formatParameters)
		{
			logger.Info(format, formatParameters);
		}

		/// <summary>
		/// Log Warn message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Warn(object message, Exception exp = null)
		{
			if (exp != null)
			{
				logger.WarnException(message.ToString(), exp);
			}
			else
			{
				logger.Warn(message);
			}
		}

		/// <summary>
		/// Log a formatted Warn message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void WarnFormat(string format, params object[] formatParameters)
		{
			logger.Warn(format, formatParameters);
		}

		/// <summary>
		/// Log Error message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Error(object message, Exception exp = null)
		{
			if (exp != null)
			{
				logger.ErrorException(message.ToString(), exp);
			}
			else
			{
				logger.Error(message);
			}
		}

		/// <summary>
		/// Log a formatted Error message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void ErrorFormat(string format, params object[] formatParameters)
		{
			logger.Error(format, formatParameters);
		}

		/// <summary>
		/// Log Fatal message
		/// </summary>
		/// <param name="message">message to log</param>
		/// <param name="exp">exception to log</param>
		public void Fatal(object message, Exception exp = null)
		{
			if (exp != null)
			{
				logger.FatalException(message.ToString(), exp);
			}
			else
			{
				logger.Fatal(message);
			}
			throw new NotImplementedException();
		}

		/// <summary>
		/// Log a formatted Fatal message
		/// </summary>
		/// <param name="format">format string</param>
		/// <param name="formatParameters">format parameters</param>
		public void FatalFormat(string format, params object[] formatParameters)
		{
			logger.Fatal(format, formatParameters);
		}
	}
}
