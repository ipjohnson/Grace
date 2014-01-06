using System;

namespace Grace.ExampleApp.DependencyInjection.ExampleClasses
{
	public interface IDisposedService : IDisposable
	{
		event EventHandler Disposed;
	}

	public class DisposedService : IDisposedService
	{
		public event EventHandler Disposed;

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}
	}
}
