using System;

namespace Grace.Logging
{
	/// <summary>
	/// Logs all message to debug console
	/// </summary>
	public class DebugConsoleLog : ILog
	{
		private readonly string logName;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="logName"></param>
		public DebugConsoleLog(string logName)
		{
			this.logName = logName;
		}

		/// <summary>
		/// Always true
		/// </summary>
		public bool IsDebugEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Always true
		/// </summary>
		public bool IsInfoEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Always true
		/// </summary>
		public bool IsWarnEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Always true
		/// </summary>
		public bool IsErrorEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Always true
		/// </summary>
		public bool IsFatalEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// Log a Debug message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Debug(object message, Exception exp = null)
		{
			Log(LogLevel.Debug, message, exp);
		}

		/// <summary>
		/// Log a Debug formatted message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void DebugFormat(string format, params object[] formatParameters)
		{
			LogFormat(LogLevel.Debug, format, formatParameters);
		}

		/// <summary>
		/// Log a Info message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Info(object message, Exception exp = null)
		{
			Log(LogLevel.Info, message, exp);
		}

		/// <summary>
		/// Log a Info formatted message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void InfoFormat(string format, params object[] formatParameters)
		{
			LogFormat(LogLevel.Info, format, formatParameters);
		}

		/// <summary>
		/// Log a Warn message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Warn(object message, Exception exp = null)
		{
			Log(LogLevel.Warn, message, exp);
		}

		/// <summary>
		/// Log a Warn formatted message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void WarnFormat(string format, params object[] formatParameters)
		{
			LogFormat(LogLevel.Warn, format, formatParameters);
		}

		/// <summary>
		/// Log an Error message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Error(object message, Exception exp = null)
		{
			Log(LogLevel.Error, message, exp);
		}

		/// <summary>
		/// Log an Error format
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void ErrorFormat(string format, params object[] formatParameters)
		{
			LogFormat(LogLevel.Error, format, formatParameters);
		}

		/// <summary>
		/// Log an Fatal message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Fatal(object message, Exception exp = null)
		{
			Log(LogLevel.Fatal, message, exp);
		}

		/// <summary>
		/// Log an Fatal format
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void FatalFormat(string format, params object[] formatParameters)
		{
			LogFormat(LogLevel.Fatal, format, formatParameters);
		}

		private void Log(LogLevel logLevel, object message, Exception exp = null)
		{
			System.Diagnostics.Debug.WriteLine("{0} {1} {2} - {3}", DateTime.Now, logLevel, logName, message);

			if (exp != null)
			{
				System.Diagnostics.Debug.WriteLine(exp.Message);
			}
		}

		private void LogFormat(LogLevel logLevel, string format, object[] formatParameters)
		{
			string formatedString = string.Format(format, formatParameters);

			System.Diagnostics.Debug.WriteLine("{0} {1} {2} - {3}", DateTime.Now, logLevel, logName, formatedString);
		}
	}
}