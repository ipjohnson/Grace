using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grace.Utilities
{
	/// <summary>
	/// Safe dictionary class, lifted shamelessly from TinyIoC
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class SafeDictionary<TKey, TValue> : IDisposable
	{
		private readonly ReaderWriterLockSlim padlock = new ReaderWriterLockSlim();
		private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

		/// <summary>
		/// Set a value into the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TValue this[TKey key]
		{
			set
			{
				padlock.EnterWriteLock();

				try
				{
					TValue current;

					if (dictionary.TryGetValue(key, out current))
					{
						var disposable = current as IDisposable;

						if (disposable != null)
							disposable.Dispose();
					}

					dictionary[key] = value;
				}
				finally
				{
					padlock.ExitWriteLock();
				}
			}
		}

		/// <summary>
		/// Try and get a key value pair from the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			padlock.EnterReadLock();
			try
			{
				return dictionary.TryGetValue(key, out value);
			}
			finally
			{
				padlock.ExitReadLock();
			}
		}

		/// <summary>
		/// Remove an entry from the dictionary
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Remove(TKey key)
		{
			padlock.EnterWriteLock();
			try
			{
				return dictionary.Remove(key);
			}
			finally
			{
				padlock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Clear the dictionary
		/// </summary>
		public void Clear()
		{
			padlock.EnterWriteLock();
			try
			{
				dictionary.Clear();
			}
			finally
			{
				padlock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Keys in the dictionary
		/// </summary>
		public IEnumerable<TKey> Keys
		{
			get
			{
				padlock.EnterReadLock();
				try
				{
					return new List<TKey>(dictionary.Keys);
				}
				finally
				{
					padlock.ExitReadLock();
				}
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Dispose of the dictionary
		/// </summary>
		public void Dispose()
		{
			padlock.EnterWriteLock();

			try
			{
				var disposableItems = from item in dictionary.Values
											 where item is IDisposable
											 select item as IDisposable;

				foreach (var item in disposableItems)
				{
					item.Dispose();
				}
			}
			finally
			{
				padlock.ExitWriteLock();
			}

			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
