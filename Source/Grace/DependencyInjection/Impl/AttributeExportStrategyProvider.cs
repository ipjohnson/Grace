using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Impl
{
	public class AttributeExportStrategyProvider : IExportStrategyProvider
	{
		private readonly IInjectionScope injectionScope;
		private readonly IEnumerable<Type> scanTypes;

		public AttributeExportStrategyProvider(IInjectionScope injectionScope,
			IEnumerable<Type> scanTypes)
		{
			this.injectionScope = injectionScope;
			this.scanTypes = scanTypes;
		}

		public IEnumerable<IExportStrategy> ProvideStrategies()
		{
			foreach (Type exportedType in scanTypes)
			{
				if (exportedType.GetTypeInfo().IsAbstract || exportedType.GetTypeInfo().IsInterface)
				{
					continue;
				}

				IEnumerable<Attribute> attributes = exportedType.GetTypeInfo().GetCustomAttributes(true);

				Attribute exportAttribute = attributes.FirstOrDefault(x => x is IExportAttribute);

				if (exportAttribute != null)
				{
					AttributeExportStrategy strategy = new AttributeExportStrategy(exportedType, attributes);

					yield return strategy;
				}
			}
		}
	}
}