using System;
using System.Collections.Generic;
using System.Linq;
using Grace.Data.Immutable;
using Grace.Logging;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Represents a disposal scope
	/// </summary>
	public class DisposalScope : IDisposalScope
	{
		private readonly object disposablesLock = new object();
		private List<Tuple<IDisposable, BeforeDisposalCleanupDelegate>> disposables;

		/// <summary>
		/// true if the object has already been disposed
		/// </summary>
		protected bool disposed;

		private ILog log;

		private ILog Log
		{
			get { return log ?? (log = Logger.GetLogger<DisposalScope>()); }
		}

		/// <summary>
		/// Dispose of this kernel and child kernels
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Add and object to the scope for disposal
		/// </summary>
		/// <param name="disposable"></param>
		/// <param name="cleanupDelegate"></param>
		public void AddDisposable(IDisposable disposable, BeforeDisposalCleanupDelegate cleanupDelegate = null)
		{
			lock (disposablesLock)
			{
				if (disposables == null)
				{
					disposables = new List<Tuple<IDisposable, BeforeDisposalCleanupDelegate>>();
				}

				disposables.Add(new Tuple<IDisposable, BeforeDisposalCleanupDelegate>(disposable, cleanupDelegate));
			}
		}

		/// <summary>
		/// Remove the object from the scope
		/// </summary>
		/// <param name="disposable"></param>
		public void RemoveDisposable(IDisposable disposable)
		{
			lock (disposablesLock)
			{
				if (disposables != null)
				{
					Tuple<IDisposable, BeforeDisposalCleanupDelegate> removeItem =
						disposables.FirstOrDefault(x => ReferenceEquals(x.Item1, disposable));

					if (removeItem != null)
					{
						disposables.Remove(removeItem);
					}
				}
			}
		}

		/// <summary>
		/// Dispose of this kernel and child kernels
		/// </summary>
		/// <param name="dispose"></param>
		protected virtual void Dispose(bool dispose)
		{
			if (disposed)
			{
				return;
			}

			if (dispose)
			{
				if (disposables == null)
				{
					disposed = true;

					return;
				}

				List<Tuple<IDisposable, BeforeDisposalCleanupDelegate>> tempList;

				lock (disposablesLock)
				{
					tempList = disposables;

					disposables = null;
				}

				if (tempList != null)
				{
					while (tempList.Count > 0)
					{
						int index = tempList.Count - 1;
						Tuple<IDisposable, BeforeDisposalCleanupDelegate> disposable = tempList[index];

						tempList.RemoveAt(index);

						try
						{
							if (disposable.Item2 != null)
							{
								disposable.Item2(disposable.Item1);
							}
						}
						catch (Exception exp)
						{
							Log.Error("Exception thrown from disposal scope", exp);
						}

						try
						{
							disposable.Item1.Dispose();
						}
						catch (Exception exp)
						{
							Log.Error("Exception thrown while disposing item", exp);
						}
					}

					disposed = true;
				}
			}
		}

		/// <summary>
		/// Returns a list of all objects being tracked
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<IDisposable> TrackedDisposables()
		{
			lock (disposablesLock)
			{
				if (disposables != null)
				{
					// I'm calling ToArray to force the select to be performed immediately rather than defering execution after the lock is gone
					return disposables.Select(x => x.Item1).ToArray();
				}

				return ImmutableArray<IDisposable>.Empty;
			}
		}
	}
}