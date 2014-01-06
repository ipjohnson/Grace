using System;
using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// THis class can be used to scope 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Owned<T> : DisposalScope where T : class
	{
		/// <summary>
		/// The export value that is owned
		/// </summary>
		public T Value { get; private set; }

		/// <summary>
		/// Sets the export value. It will only work once.
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(T value)
		{
			if (Value == null)
			{
				Value = value;

				IDisposable disposable = value as IDisposable;

				if (disposable != null)
				{
					AddDisposable(disposable);
				}
			}
		}
	}
}