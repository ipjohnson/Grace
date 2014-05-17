using System;
using System.Collections.Generic;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Injection strategy that uses attributes to define it's construction
	/// </summary>
	public class AttributedInjectionStrategy : BaseInjectionStrategy
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="injectionType"></param>
		public AttributedInjectionStrategy(Type injectionType)
			: base(injectionType)
		{
		}

		/// <summary>
		/// Initialize
		/// </summary>
		public override void Initialize()
		{
			ProcessCustomEnrichmentAttributesOnClass();

			ProcessPropertyAttributes();

			ProcessMethodAttributes();

			base.Initialize();
		}

		private void ProcessCustomEnrichmentAttributesOnClass()
		{
			foreach (Attribute attribute in Attributes)
			{
				ICustomEnrichmentExpressionAttribute enrichment = attribute as ICustomEnrichmentExpressionAttribute;

				if (enrichment != null)
				{
					ICustomEnrichmentLinqExpressionProvider provider = enrichment.GetProvider(TargeType, null);

					if (provider != null)
					{
						EnrichmentExpressionProvider(provider);
					}
				}
			}
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

					ICustomEnrichmentExpressionAttribute customEnrichmentAttribute =
						customAttribute as ICustomEnrichmentExpressionAttribute;

					if (customEnrichmentAttribute != null)
					{
						ICustomEnrichmentLinqExpressionProvider provider =
							customEnrichmentAttribute.GetProvider(TargeType, runtimeProperty);

						if (provider != null)
						{
							EnrichmentExpressionProvider(provider);
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
							ImportMethodInfo methodInfo = new ImportMethodInfo
																	{
																		MethodToImport = declaredMethod
																	};

							ImportMethod(methodInfo);

							break;
						}

						ICustomEnrichmentExpressionAttribute enrichmentAttribute = customAttribute as ICustomEnrichmentExpressionAttribute;

						if (enrichmentAttribute != null)
						{
							ICustomEnrichmentLinqExpressionProvider provider = enrichmentAttribute.GetProvider(TargeType, declaredMethod);

							if (provider != null)
							{
								EnrichmentExpressionProvider(provider);
							}
						}
					}
				}
			}
		}
	}
}