using System;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Represents a generic export strategy
	/// </summary>
	public class GenericExportStrategy : CompiledExportStrategy, IGenericExportStrategy
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="exportType"></param>
		public GenericExportStrategy(Type exportType)
			: base(exportType)
		{
		}

		/// <summary>
		/// Activate the export
		/// </summary>
		/// <param name="exportInjectionScope"></param>
		/// <param name="context"></param>
		/// <param name="consider"></param>
		/// <param name="locateKey"></param>
		/// <returns></returns>
		public override object Activate(IInjectionScope exportInjectionScope, IInjectionContext context, ExportStrategyFilter consider, object locateKey)
		{
			throw new Exception("You can't activate a generic type. The type must be closed into an compiled instance export.");
		}

		/// <summary>
		/// Creates a new closed export strategy that can be activated
		/// </summary>
		/// <param name="requestedType"></param>
		/// <returns></returns>
		public IExportStrategy CreateClosedStrategy(Type requestedType)
		{
			Type closedType = OpenGenericUtilities.CreateClosedExportTypeFromRequestingType(_exportType, requestedType);

			if (closedType != null)
			{
				TypeInfo closedTypeInfo = closedType.GetTypeInfo();
				ClosedGenericExportStrategy newExportStrategy = new ClosedGenericExportStrategy(closedType);

				if (delegateInfo.ImportProperties != null)
				{
					foreach (ImportPropertyInfo importPropertyInfo in delegateInfo.ImportProperties)
					{
						PropertyInfo propertyInfo = closedTypeInfo.GetDeclaredProperty(importPropertyInfo.Property.Name);

						ImportPropertyInfo newPropertyInfo = new ImportPropertyInfo
																		 {
																			 Property = propertyInfo,
																			 ComparerObject = importPropertyInfo.ComparerObject,
																			 ExportStrategyFilter = importPropertyInfo.ExportStrategyFilter,
																			 ImportName = importPropertyInfo.ImportName,
																			 IsRequired = importPropertyInfo.IsRequired,
																			 ValueProvider = importPropertyInfo.ValueProvider
																		 };

						newExportStrategy.ImportProperty(newPropertyInfo);
					}
				}

				if (delegateInfo.ImportMethods != null)
				{
					foreach (ImportMethodInfo importMethodInfo in delegateInfo.ImportMethods)
					{
						foreach (MethodInfo declaredMethod in closedTypeInfo.GetDeclaredMethods(importMethodInfo.MethodToImport.Name))
						{
							ParameterInfo[] importMethodParams = importMethodInfo.MethodToImport.GetParameters();
							ParameterInfo[] declaredMethodParams = declaredMethod.GetParameters();

							if (importMethodParams.Length == declaredMethodParams.Length)
							{
								ImportMethodInfo newMethodInfo = new ImportMethodInfo
																			{
																				MethodToImport = declaredMethod
																			};

								// fill in method parameters

								// TODO: match the parameters to make sure they are the same
								newExportStrategy.ImportMethod(newMethodInfo);
							}
						}
					}
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

				newExportStrategy.CreatingStrategy = this;

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