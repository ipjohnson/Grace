using System;
using System.Collections.Generic;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// This class is a simple secondary resolver implementation
	/// </summary>
	public class SimpleSecondaryExportLocator : ISecondaryExportLocator
	{
		private readonly object lockObject = new object();
		private volatile IDictionary<string, Func<object>> resolveValues;

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="initValues"></param>
		public SimpleSecondaryExportLocator(IEnumerable<KeyValuePair<string, object>> initValues = null)
		{
			resolveValues = new Dictionary<string, Func<object>>();

			if (initValues != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in initValues)
				{
					object localValue = keyValuePair.Value;

					resolveValues[keyValuePair.Key] = () => localValue;
				}
			}
		}

		/// <summary>
		/// Can locate a type
		/// </summary>
		/// <param name="context"></param>
		/// <param name="resolveName"></param>
		/// <param name="resolveType"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public bool CanLocate(IInjectionContext context, string resolveName, Type resolveType, ExportStrategyFilter consider, object locateKey)
		{
			string localName = resolveName;

			if (resolveType != null)
			{
				localName = resolveType.FullName;
			}

			Func<object> returnFunc;

			if (resolveValues.TryGetValue(localName, out returnFunc))
			{
				return true;
			}

			if (context.TargetInfo != null &&
			    resolveValues.TryGetValue(context.TargetInfo.InjectionTargetName, out returnFunc))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Resolve will be called when the injection scope can't locate a particular resource
		/// </summary>
		/// <param name="owningScope"></param>
		/// <param name="context"></param>
		/// <param name="resolveName"></param>
		/// <param name="resolveType"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object Locate(IInjectionScope owningScope, IInjectionContext context, string resolveName, Type resolveType, ExportStrategyFilter consider, object locateKey)
		{
			string localName = resolveName;

			if (resolveType != null)
			{
				localName = resolveType.FullName;
			}

			Func<object> returnFunc;

			if (resolveValues.TryGetValue(localName, out returnFunc))
			{
				return returnFunc();
			}

			if (context.TargetInfo != null &&
			    resolveValues.TryGetValue(context.TargetInfo.InjectionTargetName, out returnFunc))
			{
				return returnFunc();
			}

			return null;
		}

		/// <summary>
		/// ResolveAll will be called every time a collection is resolved
		/// </summary>
		/// <param name="owningScope"></param>
		/// <param name="context"></param>
		/// <param name="resolveName"></param>
		/// <param name="resolveType"></param>
		/// <param name="collectionEmpty"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public IEnumerable<object> LocateAll(IInjectionScope owningScope, IInjectionContext context, string resolveName, Type resolveType, bool collectionEmpty, ExportStrategyFilter consider, object locateKey)
		{
			List<object> returnValue = new List<object>();

			return returnValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resolveValue"></param>
		public void AddResolveValue<T>(T resolveValue)
		{
			lock (lockObject)
			{
				IDictionary<string, Func<object>> tempDict = new Dictionary<string, Func<object>>(resolveValues);

				tempDict[typeof(T).FullName] = () => resolveValue;

				resolveValues = tempDict;
			}
		}

		/// <summary>
		/// Adds a value to the resolver
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resolveFunc"></param>
		public void AddResolveValue<T>(Func<T> resolveFunc)
		{
			lock (lockObject)
			{
				IDictionary<string, Func<object>> tempDict = new Dictionary<string, Func<object>>(resolveValues);

				tempDict[typeof(T).FullName] = () => resolveFunc();

				resolveValues = tempDict;
			}
		}

		/// <summary>
		/// Adds a resolve value but name
		/// </summary>
		/// <param name="resolveName"></param>
		/// <param name="resolveValue"></param>
		public void AddResolveValue(string resolveName, object resolveValue)
		{
			lock (lockObject)
			{
				IDictionary<string, Func<object>> tempDict = new Dictionary<string, Func<object>>(resolveValues);

				tempDict.Add(resolveName, () => resolveValue);

				resolveValues = tempDict;
			}
		}

		/// <summary>
		/// Adds a value to the resolver
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resolveName"></param>
		/// <param name="resolveFunc"></param>
		public void AddResolveValue<T>(string resolveName, Func<T> resolveFunc)
		{
			lock (lockObject)
			{
				IDictionary<string, Func<object>> tempDict = new Dictionary<string, Func<object>>(resolveValues);

				tempDict.Add(resolveName, () => resolveFunc());

				resolveValues = tempDict;
			}
		}
	}
}