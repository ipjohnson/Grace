using System;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Represents a scope that holds disposable object
    /// </summary>
    public interface IDisposalScope
#if NETSTANDARD2_1
        : IDisposable, IAsyncDisposable
#else
        : IDisposable
#endif
    {
#if NETSTANDARD2_1
        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">object to track for async disposal</param>
        T AddAsyncDisposable<T>(T disposable)
            where T : IAsyncDisposable;

        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">object to track for async disposal</param>
        /// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
        T AddAsyncDisposable<T>(T disposable, Action<T> cleanupDelegate)
            where T : IAsyncDisposable;
#endif
        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">object to track for disposal</param>
        T AddDisposable<T>(T disposable)
            where T : IDisposable;

        /// <summary>
        /// Add an object for disposal tracking
        /// </summary>
        /// <param name="disposable">object to track for disposal</param>
        /// <param name="cleanupDelegate">logic that will be run directly before the object is disposed</param>
        T AddDisposable<T>(T disposable, Action<T> cleanupDelegate)
            where T : IDisposable;
    }
}