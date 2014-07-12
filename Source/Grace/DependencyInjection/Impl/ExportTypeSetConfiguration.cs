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
    public partial class ExportTypeSetConfiguration : IExportTypeSetConfiguration, IExportTypeSetImportPropertyConfiguration, IExportStrategyProvider
    {
        private readonly List<Func<Type, IExportCondition>> _conditions;
        private readonly List<Func<Type, bool>> _excludeClauses;
        private readonly List<Type> _exportBaseTypes;
        private readonly List<Type> _exportInterfaces;
        private readonly IInjectionScope injectionScope;
        private readonly List<Func<Type, bool>> interfaceMatchList;
        private readonly IEnumerable<Type> scanTypes;
        private readonly List<Func<Type, bool>> whereClauses;
        private readonly List<IExportStrategyInspector> inspectors;
        private readonly List<ImportGlobalPropertyInfo> importPropertiesList;
        private readonly List<WithCtorParamInfo> withCtorParams;
        private readonly List<Func<Type, IEnumerable<EnrichWithDelegate>>> enrichWithDelegates;
        private readonly List<Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>>> enrichmentProviders;
        private bool exportAllByInterface;
        private bool exportAttributedTypes;
        private bool _exportByType;
        private Func<Type, IEnumerable<Type>> exportByTypeFunc;
        private bool exportByName;
        private Func<Type, string> exportByNameFunc;
        private ExportEnvironment exportEnvironment;
        private bool externallyOwned;
        private Func<Type, int> _priorityFunc;
        private Func<Type, ILifestyle> _lifestyleFunc;
        private Func<Type, object> _withKeyFunc;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="scanTypes"></param>
        public ExportTypeSetConfiguration(IInjectionScope injectionScope, IEnumerable<Type> scanTypes)
        {
            this.injectionScope = injectionScope;
            this.scanTypes = scanTypes;

            _exportInterfaces = new List<Type>();
            _exportBaseTypes = new List<Type>();
            _conditions = new List<Func<Type, IExportCondition>>();
            _excludeClauses = new List<Func<Type, bool>>();
            whereClauses = new List<Func<Type, bool>>();
            interfaceMatchList = new List<Func<Type, bool>>();
            inspectors = new List<IExportStrategyInspector>();
            importPropertiesList = new List<ImportGlobalPropertyInfo>();
            withCtorParams = new List<WithCtorParamInfo>();
            enrichWithDelegates = new List<Func<Type, IEnumerable<EnrichWithDelegate>>>();
            enrichmentProviders = new List<Func<Type, IEnumerable<ICustomEnrichmentLinqExpressionProvider>>>();

            _lifestyleFunc = type => null;
            _priorityFunc = type => 0;
        }

        /// <summary>
        /// Provide a list of strategies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IExportStrategy> ProvideStrategies()
        {
            List<IExportStrategy> returnValues = new List<IExportStrategy>();

            if (_exportInterfaces.Count == 0 &&
                 _exportBaseTypes.Count == 0 &&
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

            if (_withKeyFunc != null)
            {
                foreach (IExportStrategy exportStrategy in returnValues)
                {
                    IConfigurableExportStrategy configurableExport = exportStrategy as IConfigurableExportStrategy;

                    if (configurableExport != null)
                    {
                        object objectKey = _withKeyFunc(configurableExport.ActivationType);

                        if (objectKey != null)
                        {
                            configurableExport.SetKey(objectKey);
                        }
                    }
                }
            }

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
            _exportInterfaces.Add(interfaceType);

            return this;
        }

        /// <summary>
        /// Export all objects that implements the specified interface
        /// </summary>
        /// <returns>returns self</returns>
        public IExportTypeSetConfiguration ByInterface<T>()
        {
            _exportInterfaces.Add(typeof(T));

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
            _exportBaseTypes.Add(baseType);

            return this;
        }

        /// <summary>
        /// Export all types based on speficied type
        /// </summary>
        /// <returns></returns>
        public IExportTypeSetConfiguration BasedOn<T>()
        {
            _exportBaseTypes.Add(typeof(T));

            return this;
        }

        /// <summary>
        /// Export the selected classes by type
        /// </summary>
        /// <param name="typeDelegate"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration ByType(Func<Type, Type> typeDelegate = null)
        {
            _exportByType = true;

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
            _lifestyleFunc = lifestyleFunc;

            return this;
        }

        /// <summary>
        /// Export with the spcified priority
        /// </summary>
        /// <param name="priority">priority to export at</param>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithPriority(int priority)
        {
            _priorityFunc = type => priority;

            return this;
        }

        /// <summary>
        /// Set priority based on a func
        /// </summary>
        /// <param name="priorityFunc"></param>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithPriority(Func<Type, int> priorityFunc)
        {
            this._priorityFunc = priorityFunc;

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
        /// Assign a lifestyle to all exports
        /// </summary>
        public LifestyleBulkConfiguration Lifestyle
        {
            get { return new LifestyleBulkConfiguration(this); }
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
            _lifestyleFunc = type => new WeakSingletonLifestyle();

            return this;
        }

        /// <summary>
        /// And condition func
        /// </summary>
        /// <param name="conditionFunc">condition picker</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration AndCondition(Func<Type, IExportCondition> conditionFunc)
        {
            _conditions.Add(conditionFunc);

            return this;
        }

        /// <summary>
        /// Export services as Singletons
        /// </summary>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration AndSingleton()
        {
            _lifestyleFunc = type => new SingletonLifestyle();

            return this;
        }

        /// <summary>
        /// Provide a func that will be used to create a key that will be used to register
        /// </summary>
        /// <param name="withKeyFunc">key func</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration WithKey(Func<Type, object> withKeyFunc)
        {
            _withKeyFunc = withKeyFunc;

            return this;
        }

        /// <summary>
        /// Set a particular life cycle 
        /// </summary>
        /// <returns></returns>
        public IExportTypeSetConfiguration WithLifestyle(ILifestyle container)
        {
            _lifestyleFunc = type => container.Clone();

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
        /// Set a particular life style
        /// </summary>
        /// <param name="container">lifestyle</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration UsingLifestyle(ILifestyle container)
        {
            _lifestyleFunc = type => container.Clone();

            return this;
        }

        /// <summary>
        /// Set a particular life style using a func
        /// </summary>
        /// <param name="lifestyleFunc">pick a lifestyle</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetConfiguration UsingLifestyle(Func<Type, ILifestyle> lifestyleFunc)
        {
            _lifestyleFunc = lifestyleFunc;

            return this;
        }

        /// <summary>
        /// Adds a condition to the export
        /// </summary>
        /// <param name="conditionDelegate"></param>
        public IExportTypeSetConfiguration When(ExportConditionDelegate conditionDelegate)
        {
            _conditions.Add((x => new WhenCondition(conditionDelegate)));

            return this;
        }

        /// <summary>
        /// Adds a condition to the export
        /// </summary>
        /// <param name="conditionDelegate"></param>
        public IExportTypeSetConfiguration Unless(ExportConditionDelegate conditionDelegate)
        {
            _conditions.Add((x => new UnlessCondition(conditionDelegate)));

            return this;
        }

        /// <summary>
        /// Adds a condition to the export
        /// </summary>
        /// <param name="condition"></param>
        public IExportTypeSetConfiguration AndCondition(IExportCondition condition)
        {
            _conditions.Add(x => condition);

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
            _excludeClauses.Add(exclude);

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



        private List<Type> FilterTypes()
        {
            List<Type> filteredTypes = new List<Type>();

            foreach (Type exportedType in scanTypes)
            {
                if (ShouldSkipType(exportedType))
                {
                    continue;
                }

                if (_excludeClauses.Count > 0)
                {
                    bool continueFlag = false;

                    foreach (Func<Type, bool> excludeClause in _excludeClauses)
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

                if (_exportBaseTypes.Count > 0)
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
                foreach (Type exportInterface in _exportInterfaces)
                {
                    if (exportInterface.GetTypeInfo().IsGenericTypeDefinition)
                    {
                        if (implementedInterface.IsConstructedGenericType &&
                             implementedInterface.GetGenericTypeDefinition() == exportInterface)
                        {
                            if (exportedType.GetTypeInfo().IsGenericTypeDefinition)
                            {
                                yield return exportInterface;
                            }
                            else
                            {
                                yield return implementedInterface;
                            }
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
            foreach (Type exportBaseType in _exportBaseTypes)
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
                exportStrategy.SetLifestyleContainer(_lifestyleFunc(exportedType));
            }

            if (externallyOwned)
            {
                exportStrategy.SetExternallyOwned();
            }

            exportStrategy.SetPriority(_priorityFunc(exportedType));
            exportStrategy.SetEnvironment(exportEnvironment);

            if (exportTypes != null)
            {
                exportTypes.Apply(t => exportStrategy.AddExportType(t));
            }

            if (exportNames != null)
            {
                exportNames.Apply(s => exportStrategy.AddExportName(s));
            }

            foreach (Func<Type, IExportCondition> conditionFunc in _conditions)
            {
                IExportCondition condition = conditionFunc(exportedType);

                if (condition != null)
                {
                    exportStrategy.AddCondition(condition);
                }
            }

            if (withCtorParams.Count > 0)
            {
                ProcessWithCtorParams(exportedType, exportStrategy);
            }

            if (importPropertiesList.Count > 0)
            {
                ProcessImportProperties(exportedType, exportStrategy);
            }

            return exportStrategy;
        }

        private void ProcessImportProperties(Type exportedType, ICompiledExportStrategy exportStrategy)
        {
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
        }

        private void ProcessWithCtorParams(Type exportedType, ICompiledExportStrategy exportStrategy)
        {
            foreach (WithCtorParamInfo withCtorParamInfo in withCtorParams)
            {
                ConstructorParamInfo constructorParamInfo = new ConstructorParamInfo
                                                            {
                                                                ParameterType = withCtorParamInfo.ParameterType,
                                                                ValueProvider = withCtorParamInfo.ValueProvider
                                                            };

                if (withCtorParamInfo.NamedFunc != null)
                {
                    constructorParamInfo.ParameterName = withCtorParamInfo.NamedFunc(exportedType);
                }

                if (withCtorParamInfo.IsRequiredFunc != null)
                {
                    constructorParamInfo.IsRequired = withCtorParamInfo.IsRequiredFunc(exportedType);
                }

                if (withCtorParamInfo.ConsiderFunc != null)
                {
                    constructorParamInfo.ExportStrategyFilter = withCtorParamInfo.ConsiderFunc(exportedType);
                }

                constructorParamInfo.LocateKeyProvider = withCtorParamInfo.LocateKeyValueProvider;

                if (withCtorParamInfo.LocateWithKeyFunc != null)
                {
                    object keyValue = withCtorParamInfo.LocateWithKeyFunc(exportedType);

                    constructorParamInfo.LocateKeyProvider = new FuncLocateKeyProvider(t => keyValue);
                }

                if (withCtorParamInfo.ImportNameFunc != null)
                {
                    constructorParamInfo.ImportName = withCtorParamInfo.ImportNameFunc(exportedType);
                }

                if (withCtorParamInfo.ValueProviderFunc != null)
                {
                    constructorParamInfo.ValueProvider = withCtorParamInfo.ValueProviderFunc(exportedType);
                }

                exportStrategy.WithCtorParam(constructorParamInfo);
            }
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