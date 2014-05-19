using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Lifestyle
{
	/// <summary>
	/// Objects that implement this interface will be listened to for disposing
	/// </summary>
	public interface INotifyWhenDisposed : IDisposable
	{
		/// <summary>
		/// Raised when the object is disposed
		/// </summary>
		event EventHandler Disposed;
	}
}
