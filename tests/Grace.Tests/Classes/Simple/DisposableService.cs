using System;
using System.Threading.Tasks;

namespace Grace.Tests.Classes.Simple
{
    public interface IDisposableService
    {
        event EventHandler<EventArgs> Disposing;
    }

    public class DisposableService : IDisposableService, IDisposable
    {
        public void Dispose()
        {
            if (Disposing != null)
            {
                Disposing(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> Disposing;
    }

#if NET5_0
    public class AsyncDisposableService : IDisposableService, IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            if (Disposing != null)
            {
                Disposing(this, EventArgs.Empty);
            }

            return ValueTask.CompletedTask;
        }

        public event EventHandler<EventArgs> Disposing;
    }
#endif
}
