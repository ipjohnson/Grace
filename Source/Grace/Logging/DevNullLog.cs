using System;

namespace Grace.Logging
{
	/// <summary>
	/// Logger that does nothing
	/// </summary>
	public class DevNullLog : ILog
	{
		/// <summary>
		/// Is Debug Log Level Enabled, always false
		/// </summary>
		public bool IsDebugEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Is Info Log Level  Enabled, always false
		/// </summary>
		public bool IsInfoEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Is Warn Log Level Enabled, always false
		/// </summary>
		public bool IsWarnEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Is Error Log Level Enabled, always false
		/// </summary>
		public bool IsErrorEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Is Fatal Log Level Enabled, always false
		/// </summary>
		public bool IsFatalEnabled
		{
			get { return false; }
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Debug(object message, Exception exp = null)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void DebugFormat(string format, params object[] formatParameters)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Info(object message, Exception exp = null)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void InfoFormat(string format, params object[] formatParameters)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Warn(object message, Exception exp = null)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void WarnFormat(string format, params object[] formatParameters)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Error(object message, Exception exp = null)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void ErrorFormat(string format, params object[] formatParameters)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exp"></param>
		public void Fatal(object message, Exception exp = null)
		{
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		/// <param name="format"></param>
		/// <param name="formatParameters"></param>
		public void FatalFormat(string format, params object[] formatParameters)
		{
		}
	}
}