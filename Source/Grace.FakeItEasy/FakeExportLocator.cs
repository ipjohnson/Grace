using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.FakeItEasy
{
	/// <summary>
	/// Locates missing interfaces using Fake It Easy
	/// </summary>
	public class FakeExportLocator : ISecondaryExportLocator
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
		/// Locates missing interfaces using Fake It Easy
		/// </summary>
		/// <param name="owningScope">owning scope</param>
		/// <param name="context">injection context</param>
		/// <param name="resolveName">resolve name</param>
		/// <param name="resolveType">resolve type</param>
		/// <param name="consider">export strategy filter</param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public object Locate(IInjectionScope owningScope, IInjectionContext context, string resolveName, Type resolveType, ExportStrategyFilter consider, object locateKey)
		{
			if (resolveType != null && resolveType.IsInterface)
			{
				Type exportStrategyType = typeof(FakeExportStrategy<>).MakeGenericType(resolveType);

				IConfigurableExportStrategy strategy = Activator.CreateInstance(exportStrategyType) as IConfigurableExportStrategy;

				if (strategy != null)
				{
					strategy.SetLifestyleContainer(new SingletonLifestyle());

					strategy.AddExportType(resolveType);

					owningScope.AddStrategy(strategy);

					return strategy.Activate(owningScope, context, consider, locateKey);
				}
			}

			return null;
		}

		/// <summary>
		/// Locates an instance of Fake type if the collection is empty
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
				Type exportStrategyType = typeof(FakeExportStrategy<>).MakeGenericType(resolveType);

				IConfigurableExportStrategy strategy = Activator.CreateInstance(exportStrategyType) as IConfigurableExportStrategy;

				if (strategy != null)
				{
					strategy.SetLifestyleContainer(new SingletonLifestyle());

					strategy.AddExportType(resolveType);

					owningScope.AddStrategy(strategy);

					yield return strategy.Activate(owningScope, context, consider,locateKey );
				}
			}
		}
	}
}