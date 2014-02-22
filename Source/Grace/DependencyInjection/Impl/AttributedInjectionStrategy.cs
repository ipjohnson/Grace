using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	public class AttributedInjectionStrategy : BaseInjectionStrategy
	{
		public AttributedInjectionStrategy(Type injectionType) : base(injectionType)
		{
		}

		public override void Initialize()
		{
			ProcessPropertyAttributes();

			ProcessMethodAttributes();

			base.Initialize();
		}

		private void ProcessPropertyAttributes()
		{
			foreach (PropertyInfo runtimeProperty in TargeType.GetRuntimeProperties())
			{
				List<IExportCondition> exportConditions = new List<IExportCondition>();
				List<IExportAttribute> exportAttributes = new List<IExportAttribute>();
				IImportAttribute importAttribute = null;
				object comparer = null;
				ExportStrategyFilter filter = null;

				foreach (Attribute customAttribute in runtimeProperty.GetCustomAttributes())
				{
					if (customAttribute is IImportAttribute)
					{
						importAttribute = customAttribute as IImportAttribute;
					}

					IImportFilterAttribute filterAttribute = customAttribute as IImportFilterAttribute;

					if (filterAttribute != null)
					{
						filter = filterAttribute.ProvideFilter(TargeType, runtimeProperty.Name);
					}

					IImportSortCollectionAttribute sortAttribute = customAttribute as IImportSortCollectionAttribute;

					if (sortAttribute != null)
					{
						comparer = sortAttribute.ProvideComparer(TargeType, runtimeProperty.Name);
					}

					IExportAttribute exportAttribute = customAttribute as IExportAttribute;

					if (exportAttribute != null)
					{
						exportAttributes.Add(exportAttribute);
					}

					IExportConditionAttribute conditionAttribute = customAttribute as IExportConditionAttribute;

					if (conditionAttribute != null)
					{
						IExportCondition condition = conditionAttribute.ProvideCondition(runtimeProperty.PropertyType);

						if (condition != null)
						{
							exportConditions.Add(condition);
						}
					}
				}

				if (importAttribute != null && runtimeProperty.CanWrite)
				{
					ImportAttributeInfo attributeInfo =
						importAttribute.ProvideImportInfo(TargeType, runtimeProperty.Name);

					if (filter == null)
					{
						filter = attributeInfo.ExportStrategyFilter;
					}

					if (comparer == null)
					{
						comparer = attributeInfo.Comparer;
					}

					if (attributeInfo.ImportKey != null)
					{
						ExportStrategyFilter keyFilter =
							(context, strategy) => IExportLocatorExtensions.CompareKeyFunction(attributeInfo.ImportKey, context, strategy);

						filter = filter != null ? 
									new ExportStrategyFilterGroup(keyFilter, filter) : keyFilter;
					}

					ImportPropertyInfo importPropertyInfo = new ImportPropertyInfo
					                                        {
						                                        ComparerObject = comparer,
						                                        ExportStrategyFilter = filter,
						                                        ImportName = attributeInfo.ImportName,
						                                        IsRequired = attributeInfo.IsRequired,
						                                        Property = runtimeProperty,
						                                        ValueProvider = attributeInfo.ValueProvider
					                                        };

					ImportProperty(importPropertyInfo);
				}
			}
		}

		private void ProcessMethodAttributes()
		{
			foreach (MethodInfo declaredMethod in TargeType.GetRuntimeMethods())
			{
				if (declaredMethod.IsPublic && !declaredMethod.IsStatic)
				{
					foreach (Attribute customAttribute in declaredMethod.GetCustomAttributes())
					{
						IImportAttribute attribute = customAttribute as IImportAttribute;

						if (attribute != null)
						{
							ImportAttributeInfo info = attribute.ProvideImportInfo(TargeType, declaredMethod.Name);

							ImportMethodInfo methodInfo = new ImportMethodInfo
							                              {
								                              MethodToImport = declaredMethod
							                              };

							ImportMethod(methodInfo);

							break;
						}
					}
				}
			}
		}
	}
}