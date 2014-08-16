using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Impl
{
	public class ClosedAttributeExportStrategy : AttributeExportStrategy
	{
		public ClosedAttributeExportStrategy(Type exportType, IEnumerable<Attribute> attributes) :
			base(exportType, attributes)
		{
		}

		/// <summary>
		/// OVerride equals to compare if to closed generics are equal
		/// </summary>
		/// <param name="obj">object to compare</param>
		/// <returns>compare value</returns>
		public override bool Equals(object obj)
		{
			ClosedAttributeExportStrategy strategy = obj as ClosedAttributeExportStrategy;

			if (strategy != null &&
			    strategy.CreatingStrategy == CreatingStrategy &&
			    strategy._exportType == _exportType)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets hashcode of activation name
		/// </summary>
		/// <returns>hash code value</returns>
		public override int GetHashCode()
		{
			return ActivationName.GetHashCode();
		}

		protected override void ProcessExportAttribute(IExportAttribute exportAttribute)
		{
			foreach (string provideExportName in exportAttribute.ProvideExportNames(_exportType))
			{
				AddExportName(provideExportName);
			}

			foreach (Type provideExportType in exportAttribute.ProvideExportTypes(_exportType))
			{
				if (provideExportType.GetTypeInfo().IsGenericTypeDefinition)
				{
					Type closedType =
						provideExportType.MakeGenericType(_exportType.GetTypeInfo().GenericTypeArguments);

					AddExportType(closedType);
				}
				else
				{
					AddExportType(provideExportType);
				}
			}
		}
	}
}