using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Represents an simple open generic export that can be closed
	/// </summary>
	public class SimpleGenericExportStrategy : ConfigurableExportStrategy, IGenericExportStrategy
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType">open type to export</param>
		public SimpleGenericExportStrategy(Type exportType)
			: base(exportType)
		{
		}

		/// <summary>
		/// Cannot activate this strategy, you must create a closed strategy
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Creates a new closed export strategy that can be activated
		/// </summary>
		/// <param name="requestingType"></param>
		/// <returns></returns>
		public IExportStrategy CreateClosedStrategy(Type requestingType)
		{
			Type closedType = OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(_exportType, requestingType);

			if(closedType != null)
			{
				SimpleExportStrategy newExportStrategy = new SimpleExportStrategy(closedType);

				foreach (string exportName in base.ExportNames)
				{
					newExportStrategy.AddExportName(exportName);
				}

				foreach (string exportName in base.ExportNames)
				{
					newExportStrategy.AddExportName(exportName);
				}

				if (_exportTypes != null)
				{
					foreach (Type type in _exportTypes)
					{
						Type newExportType = null;

						if (type.GetTypeInfo().IsInterface)
						{
							newExportType =
								closedType.GetTypeInfo()
									.ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().GUID == type.GetTypeInfo().GUID);
						}
						else
						{
							Type parentType = closedType.GetTypeInfo().BaseType;

							while (parentType != null && parentType.GetTypeInfo().GUID != type.GetTypeInfo().GUID)
							{
								parentType = parentType.GetTypeInfo().BaseType;
							}

							newExportType = parentType;
						}

						if (newExportType != null)
						{
							newExportStrategy.AddExportType(newExportType);
						}
					}
				}

				if (Lifestyle != null)
				{
					newExportStrategy.SetLifestyleContainer(Lifestyle.Clone());
				}

				if (_enrichWithDelegates != null)
				{
					foreach (var item in _enrichWithDelegates)
					{
						newExportStrategy.EnrichWithDelegate(item);
					}
				}

				foreach (var item in Metadata)
				{
					newExportStrategy.AddMetadata(item.Key, item.Value);
				}

				return newExportStrategy;
			}


			return null;
		}
	}
}