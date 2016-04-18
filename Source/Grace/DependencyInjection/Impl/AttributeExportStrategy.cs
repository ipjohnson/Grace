using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
	/// <summary>
	/// Exports a type using attributes to define how it should be exported
	/// </summary>
	public class AttributeExportStrategy : CompiledInstanceExportStrategy
	{
		private readonly IEnumerable<Attribute> classAttributes;

		/// <summary>
		/// DEfault constructor
		/// </summary>
		/// <param name="exportType"></param>
		/// <param name="attributes"></param>
		public AttributeExportStrategy(Type exportType, IEnumerable<Attribute> attributes)
			: base(exportType)
		{
			classAttributes = attributes;
		}

        public override void Initialize()
        {            
            ProcessClassAttributes();

            ProcessPropertyAttributes();

            base.Initialize();
        }

        protected override void LazyInitialize()
        {
            if(activationDelegate == null)
            {
                lock(classAttributes)
                {
                    if (activationDelegate == null)
                    {
                        ProcessConstructors();

                        ProcessMethodAttributes();

                        base.LazyInitialize();
                    }
                }
            }
        }

        private void ProcessConstructors()
		{
			if (_exportType.GetTypeInfo().DeclaredConstructors.Count() > 1)
			{
				ConstructorInfo exportConstructor = null;

				foreach (ConstructorInfo declaredConstructor in _exportType.GetTypeInfo().DeclaredConstructors)
				{
					if (declaredConstructor.GetCustomAttributes().FirstOrDefault(x => x is IImportAttribute) != null)
					{
						exportConstructor = declaredConstructor;
					}
				}

				if (exportConstructor != null)
				{
					ImportConstructor(exportConstructor);

					foreach (ParameterInfo parameterInfo in exportConstructor.GetParameters())
					{
						IImportAttribute importAttribute = null;
						ExportStrategyFilter filter = null;
						object comparer = null;

						foreach (Attribute customAttribute in parameterInfo.GetCustomAttributes())
						{
							if (customAttribute is IImportAttribute)
							{
								importAttribute = customAttribute as IImportAttribute;
							}

							if (customAttribute is IImportFilterAttribute)
							{
								filter = ((IImportFilterAttribute)customAttribute).
									ProvideFilter(parameterInfo.ParameterType, parameterInfo.Name);
							}

							if (customAttribute is IImportSortCollectionAttribute)
							{
								comparer = ((IImportSortCollectionAttribute)customAttribute).
									ProvideComparer(parameterInfo.ParameterType, parameterInfo.Name);
							}
						}

						if (importAttribute != null || filter != null || comparer != null)
						{
							IExportValueProvider valueProvider = null;
							string importName = null;
							bool isRequired = true;

							if (importAttribute != null)
							{
								ImportAttributeInfo attributeInfo =
									importAttribute.ProvideImportInfo(parameterInfo.ParameterType, parameterInfo.Name);

								if (filter == null)
								{
									filter = attributeInfo.ExportStrategyFilter;
								}

								if (comparer == null)
								{
									comparer = attributeInfo.Comparer;
								}

								importName = attributeInfo.ImportName;
								isRequired = attributeInfo.IsRequired;
								valueProvider = attributeInfo.ValueProvider;
							}

							ConstructorParamInfo constructorParamInfo = new ConstructorParamInfo
																					  {
																						  ComparerObject = comparer,
																						  ExportStrategyFilter = filter,
																						  ImportName = importName,
																						  IsRequired = isRequired,
																						  ParameterName = parameterInfo.Name,
																						  ParameterType = parameterInfo.ParameterType,
																						  ValueProvider = valueProvider
																					  };

							WithCtorParam(constructorParamInfo);
						}
					}
				}
			}
		}
        
        private void ProcessPropertyAttributes()
		{
			foreach (PropertyInfo runtimeProperty in _exportType.GetRuntimeProperties())
			{
				List<IExportCondition> exportConditions = new List<IExportCondition>();
				List<IExportAttribute> exportAttributes = new List<IExportAttribute>();
				IImportAttribute importAttribute = null;
				bool importAfterConstruction = false;
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
						filter = filterAttribute.ProvideFilter(_exportType, runtimeProperty.Name);
					}

					IImportSortCollectionAttribute sortAttribute = customAttribute as IImportSortCollectionAttribute;

					if (sortAttribute != null)
					{
						comparer = sortAttribute.ProvideComparer(_exportType, runtimeProperty.Name);
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

					IImportAfterConstructionAttribute afterConstruction = customAttribute as IImportAfterConstructionAttribute;

					if (afterConstruction != null)
					{
						importAfterConstruction = afterConstruction.ImportAfterConstruction(_exportType, runtimeProperty.PropertyType);
					}
				}

				foreach (IExportAttribute exportAttribute in exportAttributes)
				{
					IEnumerable<string> propertyExportNames = exportAttribute.ProvideExportNames(_exportType);
					IEnumerable<Type> propertyExportTypes = exportAttribute.ProvideExportTypes(_exportType);

					IExportCondition condition = null;

					if (exportConditions.Count > 0)
					{
						condition = new MultipleConditions(exportConditions.ToArray());
					}

					ExportPropertyInfo exportPropertyInfo = new ExportPropertyInfo
																		 {
																			 ExportCondition = condition,
																			 ExportNames = propertyExportNames,
																			 ExportTypes = propertyExportTypes,
																			 PropertyInfo = runtimeProperty
																		 };

					ExportProperty(exportPropertyInfo);
				}

				if (importAttribute != null && runtimeProperty.CanWrite)
				{
					ImportAttributeInfo attributeInfo =
						importAttribute.ProvideImportInfo(_exportType, runtimeProperty.Name);

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
																			 ValueProvider = attributeInfo.ValueProvider,
																			 AfterConstruction = importAfterConstruction
																		 };

					ImportProperty(importPropertyInfo);
				}
			}
		}

		private void ProcessMethodAttributes()
		{
			foreach (MethodInfo declaredMethod in _exportType.GetRuntimeMethods())
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

						if (customAttribute is IActivationCompleteAttribute)
						{
							ActivateMethod(declaredMethod);

							break;
						}

						if (customAttribute is ICustomInitializationAttribute)
						{
							break;
						}

						ICustomEnrichmentExpressionAttribute enrichmentAttribute = customAttribute as ICustomEnrichmentExpressionAttribute;

						if (enrichmentAttribute != null)
						{
							ICustomEnrichmentLinqExpressionProvider provider = enrichmentAttribute.GetProvider(ActivationType, declaredMethod);

							if (provider != null)
							{
								EnrichWithExpression(provider);
							}
						}
					}
				}
			}
		}

		protected virtual void ProcessClassAttributes()
		{
			foreach (Attribute classAttribute in classAttributes)
			{
				if (classAttribute is IExportAttribute)
				{
					ProcessExportAttribute(classAttribute as IExportAttribute);

					continue;
				}

				ILifestyleProviderAttribute provider = classAttribute as ILifestyleProviderAttribute;

				if (provider != null)
				{
					ILifestyle container = provider.ProvideLifestyle(_exportType);

					SetLifestyleContainer(container);

					continue;
				}

				IEnrichWithAttribute attribute = classAttribute as IEnrichWithAttribute;

				if (attribute != null)
				{
					IEnrichWithAttribute enrichWithAttribute = attribute;

					EnrichWithDelegate(enrichWithAttribute.ProvideDelegate(_exportType));

					continue;
				}

				IExportEnvironmentAttribute exportEnvironmentAttribute = classAttribute as IExportEnvironmentAttribute;

				if (exportEnvironmentAttribute != null)
				{
					IExportEnvironmentAttribute environmentAttribute = exportEnvironmentAttribute;

					SetEnvironment(environmentAttribute.ProvideEnvironment(_exportType));

					continue;
				}

				IExportPriorityAttribute exportPriorityAttribute = classAttribute as IExportPriorityAttribute;

				if (exportPriorityAttribute != null)
				{
					IExportPriorityAttribute priorityAttribute = exportPriorityAttribute;

					SetPriority(priorityAttribute.ProvidePriority(_exportType));
				}

				IExportKeyAttribute exportKeyAttribute = classAttribute as IExportKeyAttribute;

				if (exportKeyAttribute != null)
				{
					SetKey(exportKeyAttribute.ProvideKey(_exportType));
				}
			}
		}

		protected virtual void ProcessExportAttribute(IExportAttribute exportAttribute)
		{
			foreach (string provideExportName in exportAttribute.ProvideExportNames(_exportType))
			{
				AddExportName(provideExportName);
			}

			foreach (Type provideExportType in exportAttribute.ProvideExportTypes(_exportType))
			{
				AddExportType(provideExportType);
			}
		}
	}
}