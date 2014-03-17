using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;

namespace Grace.DependencyInjection.Impl
{
	public class GenericAttributeExportStrategy : CompiledExportStrategy, IGenericExportStrategy
	{
		private readonly IEnumerable<Attribute> attributes;

		public GenericAttributeExportStrategy(Type exportType, IEnumerable<Attribute> attributes)
			: base(exportType)
		{
			this.attributes = attributes;
		}

		public override void Initialize()
		{
			foreach (Attribute attribute in attributes)
			{
				IExportAttribute exportAttribute = attribute as IExportAttribute;

				if (exportAttribute != null)
				{
					foreach (string provideExportName in exportAttribute.ProvideExportNames(exportType))
					{
						AddExportName(provideExportName);
					}

					foreach (Type provideExportType in exportAttribute.ProvideExportTypes(exportType))
					{
						AddExportType(provideExportType);
					}
				}
			}

			base.Initialize();
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope,
			IInjectionContext context,
			ExportStrategyFilter consider)
		{
			throw new Exception("You can't activate a generic type. The type must be closed into an compiled instance export.");
		}

	
		public IExportStrategy CreateClosedStrategy(Type requestedType)
		{
			Type closedType = OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(exportType, requestedType);

			if (closedType != null)
			{
				return new ClosedAttributeExportStrategy(closedType, closedType.GetTypeInfo().GetCustomAttributes());
			}

			return null;
		}
	}
}