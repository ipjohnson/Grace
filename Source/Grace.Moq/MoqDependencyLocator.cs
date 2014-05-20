using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Lifestyle;
using Moq;

namespace Grace.Moq
{
	/// <summary>
	/// Secondary locator that resolves missing exports using Moq
	/// </summary>
	public class MoqDependencyLocator : ISecondaryExportLocator
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
			if (resolveType != null)
			{
				if (resolveType.IsConstructedGenericType && resolveType.GetGenericTypeDefinition() == typeof(Mock<>))
				{
					return true;
				}

				if (resolveType.IsAbstract || resolveType.IsInterface)
				{
					return true;
				}
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
			if (resolveType != null)
			{
				bool returnObject = false;
				Type moqType;

				if (resolveType.IsConstructedGenericType && resolveType.GetGenericTypeDefinition() == typeof(Mock<>))
				{
					moqType = typeof(MoqExportStrategy<>).MakeGenericType(resolveType.GenericTypeArguments);
				}
				else if (resolveType.IsAbstract || resolveType.IsInterface)
				{
					moqType = typeof(MoqExportStrategy<>).MakeGenericType(resolveType);

					returnObject = true;
				}
				else
				{
					return null;
				}

				IConfigurableExportStrategy newStrategy = Activator.CreateInstance(moqType) as IConfigurableExportStrategy;

				if (newStrategy != null)
				{
					newStrategy.SetLifestyleContainer(new SingletonLifestyle());

					owningScope.AddStrategy(newStrategy);

					Mock mock = newStrategy.Activate(owningScope, context, consider, locateKey) as Mock;

					if (mock == null)
					{
						return null;
					}

					return returnObject ? mock.Object : mock;
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
			if (resolveType != null && collectionEmpty)
			{
				Type moqType = typeof(MoqExportStrategy<>).MakeGenericType(resolveType);

				IConfigurableExportStrategy newStrategy = Activator.CreateInstance(moqType) as IConfigurableExportStrategy;

				if (newStrategy != null)
				{
					newStrategy.SetLifestyleContainer(new SingletonLifestyle());

					newStrategy.AddExportType(resolveType);

					newStrategy.Initialize();

					owningScope.AddStrategy(newStrategy);

					yield return newStrategy.Activate(owningScope, context, consider, locateKey);
				}
			}
		}
	}
}