using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Grace.Data;
using Grace.DependencyInjection.Attributes.Interfaces;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.CompiledExport;
using Grace.DependencyInjection.Lifestyle;
using Grace.LanguageExtensions;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Exports a set of types based on a provided configuration
    /// </summary>
    public class ExportTypeSetConfiguration : IExportTypeSetConfiguration, IExportTypeSetImportPropertyConfiguration, IExportStrategyProvider
    {
        private class ImportGlobalPropertyInfo
        {
            public Type PropertyType { get; set; }

            public string PropertyName { get; set; }

            public bool IsRequired { get; set; }

            public bool AfterConstruction { get; set; }

            public Func<Type, bool> TypeFilter { get; set; }

            public ExportStrategyFilter Consider { get; set; }

            public IExportValueProvider ValueProvider { get; set; }
        }

        private readonly List<Func<Type, IExportCondition>> conditions;
        private readonly List<Func<Type, bool>> excludeClauses;
        private readonly List<Type> exportBaseTypes;
        private readonly List<Type> exportInterfaces;
        private readonly IInjectionScope injectionScope;
        private readonly List<Func<Type, bool>> interfaceMatchList;
        private readonly IEnumerable<Type> scanTypes;
        private readonly List<Func<Type, bool>> whereClauses;
        private readonly List<IExportStrategyInspector> inspectors;
        private readonly List<ImportGlobalPropertyInfo> importPropertiesList;
        private readonly List<Func<Type, IEnumerable<EnrichWithDelegate>>> enrichWithDelegates;
        private readonly List<Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>>> enrichmentProviders;
        private bool exportAllByInterface;
        private bool exportAttributedTypes;
        private bool exportByType;
        private Func<Type, IEnumerable<Type>> exportByTypeFunc;
        private bool exportByName;
        private Func<Type, string> exportByNameFunc;
        private ExportEnvironment exportEnvironment;
        private bool externallyOwned;
        private Func<Type, int> priorityFunc;
        private Func<Type, ILifestyle> lifestyleFunc;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="scanTypes"></param>
        public ExportTypeSetConfiguration(IInjectionScope injectionScope, IEnumerable<Type> scanTypes)
        {
            this.injectionScope = injectionScope;
            this.scanTypes = scanTypes;

            exportInterfaces = new List<Type>();
            exportBaseTypes = new List<Type>();
            conditions = new List<Func<Type, IExportCondition>>();
            excludeClauses = new List<Func<Type, bool>>();
            whereClauses = new List<Func<Type, bool>>();
            interfaceMatchList = new List<Func<Type, bool>>();
            inspectors = new List<IExportStrategyInspector>();
            importPropertiesList = new List<ImportGlobalPropertyInfo>();
            enrichWithDelegates = new List<Func<Type, IEnumerable<EnrichWithDelegate>>>();
            enrichmentProviders = new List<Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>>>();

            lifestyleFunc = type => null;
            priorityFunc = type => 0;
        }

        /// <summary>
        /// Provide a list of strategies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IExportStrategy> ProvideStrategies()
        {
            List<IExportStrategy> returnValues = new List<IExportStrategy>();

            if (exportInterfaces.Count == 0 &&
                 exportBaseTypes.Count == 0 &&
                 exportByTypeFunc == null &&
                 interfaceMatchList.Count == 0 &&
                 !exportAllByInterface)
            {
                exportAttributedTypes = true;
            }

            List<Type> filteredTypes = FilterTypes();

            if (exportAttributedTypes)
            {
                AttributeExportStrategyProvider provider = new AttributeExportStrategyProvider(injectionScope,
                    filteredTypes);

                returnValues.AddRange(provider.ProvideStrategies());
            }

            returnValues.AddRange(ExportAll(filteredTypes));

            if (inspectors.Count > 0)
            {
                foreach (IExportStrategy exportStrategy in returnValues)
                {
                    IExportStrategy strategy = exportStrategy;

                    inspectors.Apply(x => x.Inspect(strategy));
                }
            }

            foreach (IExportStrategy exportStrategy in returnValues)
            {
                foreach (Func<Type, IEnumerable<EnrichWithDelegate>> enrichWithDelegate in enrichWithDelegates)
                {
                    IExportStrategy localStrategy = exportStrategy;

                    enrichWithDelegate(exportStrategy.ActivationType).Apply(localStrategy.EnrichWithDelegate);
                }

                ICompiledExportStrategy compiledExport = exportStrategy as ICompiledExportStrategy;

                if (compiledExport == null)
                {
                    continue;
                }

                foreach (Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>> enrichmentProvider in enrichmentProviders)
                {
                    enrichmentProvider(exportStrategy.ActivationType).Apply(compiledExport.EnrichWithExpression);
                }
            }

            return returnValues;
        }

        /// <summary>
        /// Export all objects that implements the specified interface
        /// </summary>
        /// <param name="interfaceType">interface type</param>
        /// <returns>returns self</returns>
        public IExportTypeSetConfiguration ByInterface(Type interfaceType)
        {
            exportInterfaces.Add(interfaceType);

            return this;
        }

        /// <summary>
        /// Export all objects that implements the specified interface
        /// </summary>
        /// <returns>returns self</returns>
        public IExportTypeSetConfiguration ByInterface<T>()
        {
            exportInterfaces.Add(typeof(T));

            return this;
        }

        /// <summary>
        /// Export all classes by interface or that match a set of interfaces
        /// </summary>
        /// <param name="filterMethod"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration ByInterfaces(Func<Type, bool> filterMethod = null)
        {
            if (filterMethod != null)
            {
                interfaceMatchList.Add(filterMethod);
            }
            else
            {
                exportAllByInterface = true;
            }

            return this;
        }

        /// <summary>
        /// Export all types based on speficied type
        /// </summary>
        /// <param name="baseType">base type to export</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration BasedOn(Type baseType)
        {
            exportBaseTypes.Add(baseType);

            return this;
        }

        /// <summary>
        /// Export all types based on speficied type
        /// </summary>
        /// <returns></returns>
        public IExportTypeSetConfiguration BasedOn<T>()
        {
            exportBaseTypes.Add(typeof(T));

            return this;
        }

        /// <summary>
        /// Export the selected classes by type
        /// </summary>
        /// <param name="typeDelegate"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration ByType(Func<Type, Type> typeDelegate = null)
        {
            exportByType = true;

            if (typeDelegate == null)
            {
                exportByTypeFunc = type => new[] { type };
            }
            else
            {
                exportByTypeFunc = type =>
                                   {
                                       var tempType = typeDelegate(type);

                                       return tempType != null ?
                                              new[] { tempType } :
                                              new Type[0];
                                   };
            }

            return this;
        }

        /// <summary>
        /// Export by a set of types for example ByTypes(t =&gt; t.GetTypeInfo().ImplementedInterfaces.Where(TypesThat.EndWith("Service")))
        /// </summary>
        /// <param name="typeDelegate"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration ByTypes(Func<Type, IEnumerable<Type>> typeDelegate)
        {
            if (typeDelegate == null)
            {
                throw new ArgumentNullException("typeDelegate");
            }

            exportByTypeFunc = typeDelegate;

            return this;
        }

        /// <summary>
        /// Export by a particular name 
        /// </summary>
        /// <param name="nameDelegate">delegate used to create export name, default is type => type.Name</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration ByName(Func<Type, string> nameDelegate = null)
        {
            exportByName = true;

            exportByNameFunc = nameDelegate ?? (type => type.Name);

            return this;
        }

        /// <summary>
        /// Set a particular life style using a func
        /// </summary>
        /// <param name="lifestyleFunc">pick a lifestyle</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration WithLifestyle(Func<Type, ILifestyle> lifestyleFunc)
        {
            this.lifestyleFunc = lifestyleFunc;

            return this;
        }

        /// <summary>
        /// Export with the spcified priority
        /// </summary>
        /// <param name="priority">priority to export at</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithPriority(int priority)
        {
            priorityFunc = type => priority;

            return this;
        }

        /// <summary>
        /// Set priority based on a func
        /// </summary>
        /// <param name="priorityFunc"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithPriority(Func<Type, int> priorityFunc)
        {
            this.priorityFunc = priorityFunc;

            return this;
        }

        /// <summary>
        /// Priority will be set using a priority attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration WithPriorityAttribute<T>() where T : Attribute, IExportPriorityAttribute
        {
            inspectors.Add(new PriorityAttributeInspector<T>());

            return this;
        }

        /// <summary>
        /// Export in the specified Environment
        /// </summary>
        /// <param name="environment">environment to export in</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration InEnvironment(ExportEnvironment environment)
        {
            exportEnvironment = environment;

            return this;
        }

        /// <summary>
        /// Mark all exports as externally owned
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration ExternallyOwned()
        {
            externallyOwned = true;

            return this;
        }

        /// <summary>
        /// Exports are to be marked as shared, similar to a singleton only using a weak reference.
        /// It can not be of type IDisposable
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration AndWeakSingleton()
        {
            lifestyleFunc = type => new WeakSingletonLifestyle();

            return this;
        }

        /// <summary>
        /// And condition func
        /// </summary>
        /// <param name="conditionFunc">condition picker</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration AndCondition(Func<Type, IExportCondition> conditionFunc)
        {
            conditions.Add(conditionFunc);

            return this;
        }

        /// <summary>
        /// Export services as Singletons
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration AndSingleton()
        {
            lifestyleFunc = type => new SingletonLifestyle();

            return this;
        }

        /// <summary>
        /// Set a particular life cycle 
        /// </summary>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithLifestyle(ILifestyle container)
        {
            lifestyleFunc = type => container.Clone();

            return this;
        }

        /// <summary>
        /// Export all attributed types
        /// </summary>
        /// <returns></returns>
        public IExportTypeSetConfiguration ExportAttributedTypes()
        {
            exportAttributedTypes = true;

            return this;
        }

        /// <summary>
        /// Adds a condition to the export
        /// </summary>
        /// <param name="conditionDelegate"></param>
        public IExportTypeSetConfiguration When(ExportConditionDelegate conditionDelegate)
        {
            conditions.Add((x => new WhenCondition(conditionDelegate)));

            return this;
        }

        /// <summary>
        /// Adds a condition to the export
        /// </summary>
        /// <param name="conditionDelegate"></param>
        public IExportTypeSetConfiguration Unless(ExportConditionDelegate conditionDelegate)
        {
            conditions.Add((x => new UnlessCondition(conditionDelegate)));

            return this;
        }

        /// <summary>
        /// Adds a condition to the export
        /// </summary>
        /// <param name="condition"></param>
        public IExportTypeSetConfiguration AndCondition(IExportCondition condition)
        {
            conditions.Add(x => condition);

            return this;
        }

        /// <summary>
        /// Enrich with linq expressions
        /// </summary>
        /// <param name="providers">providers</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration EnrichWithExpression(Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>> providers)
        {
            this.enrichmentProviders.Add(providers);

            return this;
        }

        /// <summary>
        /// Exclude a type from being used
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration Exclude(Func<Type, bool> exclude)
        {
            excludeClauses.Add(exclude);

            return this;
        }

        /// <summary>
        /// Allows you to filter out types based on the provided where clause
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration Select(Func<Type, bool> whereClause)
        {
            whereClauses.Add(whereClause);

            return this;
        }

        /// <summary>
        /// Adds a new inspector to the export configuration
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithInspector(IExportStrategyInspector inspector)
        {
            inspectors.Add(inspector);

            return this;
        }

        /// <summary>
        /// Enrich all with a particular delegate
        /// </summary>
        /// <param name="enrichWithDelegate">enrichment delegate</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration EnrichWith(EnrichWithDelegate enrichWithDelegate)
        {
            enrichWithDelegates.Add(t => new[] { enrichWithDelegate });

            return this;
        }

        /// <summary>
        /// Enrich with linq statements
        /// </summary>
        /// <param name="enrichWithDelegates">linq statement picker</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration EnrichWith(Func<Type, IEnumerable<EnrichWithDelegate>> enrichWithDelegates)
        {
            this.enrichWithDelegates.Add(enrichWithDelegates);

            return this;
        }

        /// <summary>
        /// Enrich all with linq expressions
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration EnrichWithExpression(ICustomEnrichmentLinqExpressionProvider provider)
        {
            enrichmentProviders.Add(type => new[] { provider });

            return this;
        }

        /// <summary>
        /// Import properties of type TProperty and by name
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <returns>
        /// configuration object
        /// </returns>
        public IExportTypeSetImportPropertyConfiguration ImportProperty<TProperty>()
        {
            importPropertiesList.Add(new ImportGlobalPropertyInfo { PropertyType = typeof(TProperty), IsRequired = true });

            return this;
        }

        /// <summary>
        /// Import all properties that match the type
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public IExportTypeSetImportPropertyConfiguration ImportProperty(Type propertyType)
        {
            importPropertiesList.Add(new ImportGlobalPropertyInfo { PropertyType = propertyType, IsRequired = true });

            return this;
        }

        /// <summary>
        /// Property Name to import
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>
        /// configuration object
        /// </returns>
        public IExportTypeSetImportPropertyConfiguration Named(string propertyName)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].PropertyName = propertyName;
            }

            return this;
        }

        /// <summary>
        /// Is it required
        /// </summary>
        /// <param name="value">is required</param>
        /// <returns>
        /// configuration object
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IExportTypeSetImportPropertyConfiguration IsRequired(bool value)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].IsRequired = value;
            }

            return this;
        }

        /// <summary>
        /// Apply delegate to choose export
        /// </summary>
        /// <param name="consider">consider filter</param>
        /// <returns>
        /// configuration object
        /// </returns>
        public IExportTypeSetImportPropertyConfiguration Consider(ExportStrategyFilter consider)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].Consider = consider;
            }

            return this;
        }

        /// <summary>
        /// Using Value provider
        /// </summary>
        /// <param name="activationDelegate"></param>
        /// <returns>
        /// configuration object
        /// </returns>
        public IExportTypeSetImportPropertyConfiguration UsingValue(ExportActivationDelegate activationDelegate)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].ValueProvider = new ExportActivationValueProvider(activationDelegate);
            }

            return this;
        }

        /// <summary>
        /// Use value provider
        /// </summary>
        /// <param name="valueProvider">value provider</param>
        /// <returns>
        /// configuration object
        /// </returns>
        public IExportTypeSetImportPropertyConfiguration UsingValueProvider(IExportValueProvider valueProvider)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].ValueProvider = valueProvider;
            }

            return this;
        }

        public IExportTypeSetImportPropertyConfiguration AfterConstruction()
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].AfterConstruction = true;
            }

            return this;
        }

        public IExportTypeSetImportPropertyConfiguration OnlyOn(Func<Type, bool> filter)
        {
            if (importPropertiesList.Count > 0)
            {
                importPropertiesList[importPropertiesList.Count - 1].TypeFilter = filter;
            }

            return this;
        }

        private List<Type> FilterTypes()
        {
            List<Type> filteredTypes = new List<Type>();

            foreach (Type exportedType in scanTypes)
            {
                if (ShouldSkipType(exportedType))
                {
                    continue;
                }

                if (excludeClauses.Count > 0)
                {
                    bool continueFlag = false;

                    foreach (Func<Type, bool> excludeClause in excludeClauses)
                    {
                        if (excludeClause(exportedType))
                        {
                            continueFlag = true;
                        }
                    }

                    if (continueFlag)
                    {
                        continue;
                    }
                }

                if (whereClauses.Count > 0)
                {
                    bool continueFlag = true;

                    foreach (Func<Type, bool> whereClause in whereClauses)
                    {
                        if (whereClause(exportedType))
                        {
                            continueFlag = false;
                            break;
                        }
                    }

                    if (continueFlag)
                    {
                        continue;
                    }
                }

                if (exportBaseTypes.Count > 0)
                {
                    Type matchedType;

                    if (!MatchesExportBaseType(exportedType, out matchedType))
                    {
                        continue;
                    }
                }

                filteredTypes.Add(exportedType);
            }

            return filteredTypes;
        }

        private static bool ShouldSkipType(Type exportedType)
        {
            return exportedType.GetTypeInfo().IsInterface ||
                     exportedType.GetTypeInfo().IsAbstract ||
                     typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(exportedType.GetTypeInfo()) ||
                     typeof(Exception).GetTypeInfo().IsAssignableFrom(exportedType.GetTypeInfo());
        }

        private IEnumerable<Type> ScanTypeForExportedInterfaces(Type filteredType)
        {
            List<Type> returnValue = new List<Type>();

            if (exportAllByInterface)
            {
                returnValue.AddRange(ExportTypeByInterfaces(filteredType));
            }

            returnValue.AddRange(ExportTypeIfMatchesInterface(filteredType));

            return returnValue;
        }

        private IEnumerable<IExportStrategy> ExportAll(IEnumerable<Type> filteredTypes)
        {
            foreach (Type filteredType in filteredTypes)
            {
                if (ShouldSkipType(filteredType))
                {
                    continue;
                }

                List<Type> exportTypes = new List<Type>();
                List<string> exportNames = new List<string>();
                bool isGeneric = filteredType.GetTypeInfo().IsGenericTypeDefinition;

                if (exportByTypeFunc != null)
                {
                    exportTypes.AddRange(exportByTypeFunc(filteredType).ToArray());
                }

                if (exportByNameFunc != null)
                {
                    exportNames.Add(exportByNameFunc(filteredType));
                }

                exportTypes.AddRange(ScanTypeForExportedInterfaces(filteredType));

                if (exportTypes.Count > 0)
                {
                    yield return CreateCompiledExportStrategy(filteredType, isGeneric, exportTypes, exportNames);
                }
            }
        }

        private IEnumerable<Type> ExportTypeByInterfaces(Type exportedType)
        {
            bool generic = false;
            List<Type> exportTypes = new List<Type>();
            IExportStrategy returnValue = null;

            if (exportedType.GetTypeInfo().IsGenericTypeDefinition)
            {
                Type[] genericArgs = exportedType.GetTypeInfo().GenericTypeParameters;

                foreach (Type implementedInterface in exportedType.GetTypeInfo().ImplementedInterfaces)
                {
                    if (ShouldSkipExportInterface(implementedInterface))
                    {
                        continue;
                    }

                    if (implementedInterface.IsConstructedGenericType)
                    {
                        exportTypes.Add(implementedInterface.GetGenericTypeDefinition());
                    }
                }

                generic = true;
            }
            else
            {
                exportTypes.AddRange(exportedType.GetTypeInfo().ImplementedInterfaces.Where(x => !ShouldSkipExportInterface(x)));
            }

            return exportTypes;
        }

        private IEnumerable<Type> ExportTypeIfMatchesInterface(Type exportedType)
        {
            if (exportedType.GetTypeInfo().IsGenericTypeDefinition)
            {
                return ExportGenericTypeIfMatchesInterface(exportedType);
            }

            return ExportNonGenericTypeIfMatchesInterface(exportedType);
        }

        private IEnumerable<Type> ExportGenericTypeIfMatchesInterface(Type exportedType)
        {
            List<Type> exportTypes = new List<Type>();
            Type matchingType;

            bool exportAsSelfAndBase = MatchesExportBaseType(exportedType, out matchingType);

            exportTypes.AddRange(FindMatchingExportInterfaces(exportedType));

            if (exportAsSelfAndBase || exportTypes.Count > 0)
            {
                if (exportAsSelfAndBase)
                {
                    exportTypes.Add(matchingType.GetGenericTypeDefinition());
                    exportTypes.Add(exportedType);
                }
            }

            return exportTypes;
        }

        private IEnumerable<Type> ExportNonGenericTypeIfMatchesInterface(Type exportedType)
        {
            List<Type> exportTypes = new List<Type>();
            Type matchingType;

            bool exportAsSelf = MatchesExportBaseType(exportedType, out matchingType);

            exportTypes.AddRange(FindMatchingExportInterfaces(exportedType));

            if (exportAsSelf || exportTypes.Count > 0)
            {
                if (exportAsSelf)
                {
                    exportTypes.Add(matchingType);
                    exportTypes.Add(exportedType);
                }
            }

            return exportTypes;
        }

        private IEnumerable<Type> FindMatchingExportInterfaces(Type exportedType)
        {
            foreach (Type implementedInterface in exportedType.GetTypeInfo().ImplementedInterfaces)
            {
                foreach (Type exportInterface in exportInterfaces)
                {
                    if (exportInterface.GetTypeInfo().IsGenericTypeDefinition)
                    {
                        if (implementedInterface.IsConstructedGenericType &&
                             implementedInterface.GetGenericTypeDefinition() == exportInterface)
                        {
                            yield return exportInterface;
                        }
                    }
                    else if (exportInterface.GetTypeInfo().IsAssignableFrom(implementedInterface.GetTypeInfo()))
                    {
                        yield return exportInterface;
                    }
                }

                foreach (Func<Type, bool> func in interfaceMatchList)
                {
                    if (func(implementedInterface))
                    {
                        yield return implementedInterface;
                    }
                }
            }
        }

        private bool ShouldSkipExportInterface(Type exportInterface)
        {
            if (exportInterface == typeof(IDisposable))
            {
                return true;
            }

            Type genericInterface = exportInterface;

            if (exportInterface.IsConstructedGenericType)
            {
                genericInterface = exportInterface.GetGenericTypeDefinition();
            }

            if (genericInterface == typeof(IEnumerable<>) ||
                 genericInterface == typeof(ICollection<>) ||
                 genericInterface == typeof(IList<>) ||
                 genericInterface == typeof(IDictionary<,>))
            {
                return true;
            }

            return false;
        }

        private bool MatchesExportBaseType(Type exportedType, out Type matchingType)
        {
            foreach (Type exportBaseType in exportBaseTypes)
            {
                if (ReflectionService.CheckTypeIsBasedOnAnotherType(exportedType, exportBaseType))
                {
                    matchingType = exportBaseType;

                    return true;
                }
            }

            matchingType = null;

            return false;
        }

        private ICompiledExportStrategy CreateCompiledExportStrategy(Type exportedType,
            bool generic,
            IEnumerable<Type> exportTypes,
            IEnumerable<string> exportNames)
        {
            ICompiledExportStrategy exportStrategy;

            if (generic)
            {
                exportStrategy = new GenericExportStrategy(exportedType);
            }
            else
            {
                exportStrategy = new AttributeExportStrategy(exportedType, exportedType.GetTypeInfo().GetCustomAttributes(true));
            }

            if (exportStrategy.Lifestyle == null)
            {
                exportStrategy.SetLifestyleContainer(lifestyleFunc(exportedType));
            }

            if (externallyOwned)
            {
                exportStrategy.SetExternallyOwned();
            }

            exportStrategy.SetPriority(priorityFunc(exportedType));
            exportStrategy.SetEnvironment(exportEnvironment);

            if (exportTypes != null)
            {
                exportTypes.Apply(t => exportStrategy.AddExportType(t));
            }

            if (exportNames != null)
            {
                exportNames.Apply(s => exportStrategy.AddExportName(s));
            }

            foreach (Func<Type, IExportCondition> conditionFunc in conditions)
            {
                IExportCondition condition = conditionFunc(exportedType);

                if (condition != null)
                {
                    exportStrategy.AddCondition(condition);
                }
            }

            foreach (ImportGlobalPropertyInfo importProperty in importPropertiesList)
            {
                foreach (PropertyInfo runtimeProperty in exportedType.GetRuntimeProperties())
                {
                    if (runtimeProperty.CanWrite &&
                         !runtimeProperty.SetMethod.IsStatic &&
                         runtimeProperty.SetMethod.IsPublic &&
                         CheckPropertyTypesMatch(exportedType, runtimeProperty.PropertyType, importProperty))
                    {
                        if (importProperty.PropertyName == null ||
                             importProperty.PropertyName.ToLowerInvariant() == runtimeProperty.Name.ToLowerInvariant())
                        {
                            exportStrategy.ImportProperty(new ImportPropertyInfo
                            {
                                Property = runtimeProperty,
                                IsRequired = importProperty.IsRequired,
                                ValueProvider = importProperty.ValueProvider,
                                ExportStrategyFilter = importProperty.Consider,
                                AfterConstruction = importProperty.AfterConstruction
                            });
                        }
                    }
                }
            }

            return exportStrategy;
        }

        private bool CheckPropertyTypesMatch(Type exportedType, Type importType, ImportGlobalPropertyInfo importProperty)
        {
            if (importProperty.TypeFilter != null &&
                !importProperty.TypeFilter(exportedType))
            {
                return false;
            }

            if (importType.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (importProperty.PropertyType.GetTypeInfo().IsGenericTypeDefinition &&
                     importProperty.PropertyType.GetTypeInfo().GUID == importType.GetTypeInfo().GUID)
                {
                    return true;
                }

                if (importProperty.PropertyType.IsConstructedGenericType &&
                     importProperty.PropertyType.GetTypeInfo().GetGenericTypeDefinition().GetTypeInfo().GUID == importType.GetTypeInfo().GUID)
                {
                    return true;
                }
            }
            else
            {
                return importProperty.PropertyType.GetTypeInfo().IsAssignableFrom(importType.GetTypeInfo());
            }

            return false;
        }
    }
}