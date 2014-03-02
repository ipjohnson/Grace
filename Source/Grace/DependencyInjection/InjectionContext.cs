using System;
using System.Collections;
using System.Collections.Generic;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection
{
	/// <summary>
	/// Default implementation of IInjectionContext
	/// </summary>
	public class InjectionContext : IInjectionContext, IEnumerable<KeyValuePair<string, ExportActivationDelegate>>
	{
		private Dictionary<string, ExportActivationDelegate> exportsByName;
		private Dictionary<Type, ExportActivationDelegate> exportsByType;
		private Dictionary<string, object> extraData;
		private int resolveDepth;

		/// <summary>
		/// Constructor that uses requesting scope as disposal scope
		/// </summary>
		/// <param name="requestingScope"></param>
		public InjectionContext(IInjectionScope requestingScope) :
			this(requestingScope, requestingScope)
		{
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="disposalScope"></param>
		/// <param name="requestingScope"></param>
		public InjectionContext(IDisposalScope disposalScope, IInjectionScope requestingScope)
		{
			MaxResolveDepth = 150;
			DisposalScope = disposalScope;
			RequestingScope = requestingScope;
		}

		/// <summary>
		/// Returns an enumeration of exports
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<string, ExportActivationDelegate>> GetEnumerator()
		{
			if (exportsByName != null)
			{
				foreach (KeyValuePair<string, ExportActivationDelegate> exportActivationDelegate in exportsByName)
				{
					yield return exportActivationDelegate;
				}
			}
		}

		/// <summary>
		/// Gets an enumation of exports
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IInjectionContext Clone()
		{
			InjectionContext injectionContext = new InjectionContext(DisposalScope, RequestingScope);

			if (exportsByName != null)
			{
				injectionContext.exportsByName = new Dictionary<string, ExportActivationDelegate>(exportsByName);
			}

			if (exportsByType != null)
			{
				injectionContext.exportsByType = new Dictionary<Type, ExportActivationDelegate>(exportsByType);
			}

			if (extraData != null)
			{
				injectionContext.extraData = new Dictionary<string, object>(extraData);
			}

			injectionContext.TargetInfo = TargetInfo;
			injectionContext.resolveDepth = resolveDepth;
			injectionContext.RequestingScope = RequestingScope;
			injectionContext.DisposalScope = DisposalScope;

			return injectionContext;
		}

		/// <summary>
		/// Disposal scope for the injection context
		/// </summary>
		public IDisposalScope DisposalScope { get; set; }

		/// <summary>
		/// The scope that the request originated in
		/// </summary>
		public IInjectionScope RequestingScope { get; set; }

		/// <summary>
		/// The target information for the current injection
		/// </summary>
		public IInjectionTargetInfo TargetInfo { get; set; }

		/// <summary>
		/// Extra data associated with the injection request. 
		/// </summary>
		/// <param name="dataName"></param>
		/// <returns></returns>
		public object GetExtraData(string dataName)
		{
			object returnValue = null;

			if (extraData != null)
			{
				extraData.TryGetValue(dataName, out returnValue);
			}

			return returnValue;
		}

		/// <summary>
		/// Sets extra data on the injection context
		/// </summary>
		/// <param name="dataName"></param>
		/// <param name="newValue"></param>
		public void SetExtraData(string dataName, object newValue)
		{
			if (extraData == null)
			{
				extraData = new Dictionary<string, object>();
			}

			extraData[dataName] = newValue;
		}

		/// <summary>
		/// Locate an export by type
		/// </summary>
		/// <returns></returns>
		public object Locate(Type type)
		{
			if (exportsByType != null)
			{
				ExportActivationDelegate activationDelegate;

				if (exportsByType.TryGetValue(type, out activationDelegate))
				{
					return activationDelegate(RequestingScope, this);
				}
			}

			return null;
		}

		/// <summary>
		/// Locate an export by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object Locate(string name)
		{
			if (exportsByName != null)
			{
				ExportActivationDelegate activationDelegate;

				if (exportsByName.TryGetValue(name.ToLowerInvariant(), out activationDelegate))
				{
					return activationDelegate(RequestingScope, this);
				}
			}

			return null;
		}

		/// <summary>
		/// Register an export by type for this injection context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exportFunction"></param>
		public void Export<T>(ExportFunction<T> exportFunction)
		{
			Export(typeof(T), (x, y) => exportFunction(x, y));
		}

		/// <summary>
		/// Export by type
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="activationDelegate"></param>
		public void Export(Type exportType, ExportActivationDelegate activationDelegate)
		{
			if (exportsByType == null)
			{
				exportsByType = new Dictionary<Type, ExportActivationDelegate>();
			}

			exportsByType[exportType] = activationDelegate;
		}

		/// <summary>
		/// Register an export by name for this injection context
		/// </summary>
		/// <param name="name"></param>
		/// <param name="activationDelegate"></param>
		public void Export(string name, ExportActivationDelegate activationDelegate)
		{
			name = name.ToLowerInvariant();

			if (exportsByName == null)
			{
				exportsByName = new Dictionary<string, ExportActivationDelegate>();
			}

			exportsByName[name] = activationDelegate;
		}

		/// <summary>
		/// Max resolve depth allowed
		/// </summary>
		public int MaxResolveDepth { get; set; }

		/// <summary>
		/// Increment the resolve depth by one
		/// </summary>
		public void IncrementResolveDepth()
		{
			resolveDepth++;

			if (resolveDepth > MaxResolveDepth)
			{
				if (TargetInfo != null)
				{
					throw new CircularDependencyDetectedException(TargetInfo.LocateName, TargetInfo.LocateType, this);
				}
				else
				{
					throw new CircularDependencyDetectedException(null, null, this);
				}
			}
		}

		/// <summary>
		/// Decrement the resolve depth by one
		/// </summary>
		public void DecrementResolveDepth()
		{
			resolveDepth--;
		}

		/// <summary>
		/// Add a new object to injection context for export
		/// </summary>
		/// <param name="export"></param>
		public void Add(object export)
		{
			Export(export.GetType(), (x, y) => export);
		}

		/// <summary>
		/// Add a new Type to injection context for export
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exportFunc"></param>
		public void Add<T>(ExportFunction<T> exportFunc)
		{
			Export(exportFunc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(string key, object value)
		{
			Add(key.ToLowerInvariant(), (x, y) => value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="activationDelegate"></param>
		public void Add(string key, ExportActivationDelegate activationDelegate)
		{
			Export(key.ToLowerInvariant(), activationDelegate);
		}
	}
}