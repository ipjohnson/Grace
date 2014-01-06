using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Grace.Logging;

namespace Grace.Data
{
	/// <summary>
	/// A base object that implements INotifyPropertyChanged and offers logging
	/// </summary>
	public class NotifyObject : INotifyPropertyChanged
	{
		private ILog log;

		/// <summary>
		/// ILog instance for this class
		/// </summary>
		protected ILog Log
		{
			get { return log ?? (log = Logger.GetLogger(GetType())); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Sets a value to a particular property while calling PropertyChanged
		/// Note: Default implementation from VS temlate
		/// </summary>
		/// <typeparam name="T">type of property</typeparam>
		/// <param name="storage">backing field for the property</param>
		/// <param name="value">new value to set</param>
		/// <param name="propertyName">property name (usually left blank if calling from property)</param>
		/// <returns>true if the value was set (false if its the same value)</returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
		{
			if (Equals(storage, value))
			{
				return false;
			}

			storage = value;

			OnPropertyChanged(propertyName);

			return true;
		}

		/// <summary>
		/// Default implementation of Propertychanged event invoker
		/// </summary>
		/// <param name="propertyName">property that changed</param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}