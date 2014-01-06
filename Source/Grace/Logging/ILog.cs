using System;
using JetBrains.Annotations;

namespace Grace.Logging
{
	/// <summary>
	/// an instance of a log
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Is Debug Log Level Enabled
		/// </summary>
		bool IsDebugEnabled { get; }

		/// <summary>
		/// Is Info Log Level  Enabled
		/// </summary>
		bool IsInfoEnabled { get; }

		/// <summary>
		/// Is Warn Log Level Enabled
		/// </summary>
		bool IsWarnEnabled { get; }

		/// <summary>
		/// Is Error Log Level Enabled
		/// </summary>
		bool IsErrorEnabled { get; }

		/// <summary>
		/// Is Fatal Log Level Enabled
		/// </summary>
		bool IsFatalEnabled { get; }

		/// <summary>
		/// Log a Debug message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		void Debug([NotNull]object message, Exception exp = null);

		/// <summary>
		/// Log a Debug formatted message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		[StringFormatMethod("format")]
		void DebugFormat(string format, params object[] formatParameters);

		/// <summary>
		/// Log a Info message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		void Info([NotNull]object message, Exception exp = null);

		/// <summary>
		/// Log a Info formatted message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		[StringFormatMethod("format")]
		void InfoFormat(string format, params object[] formatParameters);

		/// <summary>
		/// Log a Warn message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		void Warn([NotNull]object message, Exception exp = null);

		/// <summary>
		/// Log a Warn formatted message
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		[StringFormatMethod("format")]
		void WarnFormat(string format, params object[] formatParameters);

		/// <summary>
		/// Log an Error message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		void Error([NotNull]object message, Exception exp = null);

		/// <summary>
		/// Log an Error format
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		[StringFormatMethod("format")]
		void ErrorFormat(string format, params object[] formatParameters);

		/// <summary>
		/// Log an Fatal message
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		void Fatal([NotNull]object message, Exception exp = null);

		/// <summary>
		/// Log an Fatal format
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		[StringFormatMethod("format")]
		void FatalFormat(string format, params object[] formatParameters);
	}
}