using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	public class ConcreteAttributeExportStrategy : AttributeExportStrategy
	{
		public ConcreteAttributeExportStrategy(Type exportType)
			: base(exportType,exportType.GetTypeInfo().GetCustomAttributes(true))
		{
			
		}

		public ConcreteAttributeExportStrategy(Type exportType, IEnumerable<Attribute> attributes)
			: base(exportType, attributes)
		{
		}

		public override bool Equals(object obj)
		{
			ConcreteAttributeExportStrategy strategy = obj as ConcreteAttributeExportStrategy;

			if (strategy != null &&
			    strategy.exportType == exportType &&
			    OwningScope == strategy.OwningScope)
			{
				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ActivationName.GetHashCode();
		}
	}
}