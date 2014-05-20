using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.NSubstitute
{
	/// <summary>
	/// Dependency Locator that creates objects using NSubstitute
	/// </summary>
	public class NSubstituteDependencyLocator : ISecondaryExportLocator
	{
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
			if (resolveType != null && resolveType.IsInterface)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resolves types using NSubstitute
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
			if (resolveType != null && resolveType.IsInterface)
			{
				Type createType = typeof(NSubstituteExportStrategy<>).MakeGenericType(resolveType);

				ICompiledExportStrategy newStrategy = Activator.CreateInstance(createType) as ICompiledExportStrategy;

				if (newStrategy != null)
				{
					newStrategy.SetLifestyleContainer(new SingletonLifestyle());

					newStrategy.AddExportType(resolveType);

					owningScope.AddStrategy(newStrategy);

					return newStrategy.Activate(owningScope, context, null, locateKey);
				}
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
			if (resolveType != null && resolveType.IsInterface && collectionEmpty)
			{
				Type createType = typeof(NSubstituteExportStrategy<>).MakeGenericType(resolveType);

				ICompiledExportStrategy newStrategy = Activator.CreateInstance(createType) as ICompiledExportStrategy;

				if (newStrategy != null)
				{
					newStrategy.SetLifestyleContainer(new SingletonLifestyle());

					newStrategy.AddExportType(resolveType);

					owningScope.AddStrategy(newStrategy);

					yield return newStrategy.Activate(owningScope, context, null, locateKey);
				}
			}
		}
	}
}