using System;
using Grace.DependencyInjection.Impl;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Impl.Wrappers;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Configuration for injection scope
    /// </summary>
    public class InjectionScopeConfiguration : IInjectionScopeConfiguration
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public InjectionScopeConfiguration(ImplementationFactory implementationFactory = null)
        {
            Implementation = implementationFactory ?? DefaultImplementation.Clone();
            Behaviors = new ExportCompilationBehavior();

            CacheArraySize = 64;
            ExportStrategyArraySize = 16;
            AutoRegisterUnknown = true;
            ExportAsBase = false;
            TrackDisposableTransients = true;
            SupportFuncType = true;
            InjectIDisposable = true;
            ReturnKeyedInEnumerable = false;

            ExportByInterfaceFilter = DefaultInterfaceFilter;
        }

        /// <summary>
        /// internal method used by the container
        /// </summary>
        /// <param name="scope">owning scope</param>
        void IInjectionScopeConfiguration.SetInjectionScope(IInjectionScope scope)
        {
            Implementation.InjectionScope = scope;
        }

        /// <summary>
        /// Allows you to configure how to construct compiled exports.
        /// </summary>
        public ExportCompilationBehavior Behaviors { get; protected set; }

        /// <summary>
        /// Catch exceptions on disposal, false by default
        /// </summary>
        public bool CatchDisposalExceptions { get; set; }

        /// <summary>
        /// This the containers internal DI container. If you want to change any implementation you would add them here
        /// </summary>
        public ImplementationFactory Implementation { get; protected set; }

        /// <summary>
        /// Size of the array used to cache execution delegates. By default it's 64, if you wish to change this make sure it's a positive power of 2
        /// </summary>
        public int CacheArraySize { get; set; }

        /// <summary>
        /// Size of array used to cache export strategies. By default it's 16, if you wish to change this make sure it's a positive power of 2
        /// </summary>
        public int ExportStrategyArraySize { get; set; }

        /// <summary>
        /// Register concrete implementation that are unknown
        /// </summary>
        public bool AutoRegisterUnknown { get; set; }

        /// <summary>
        /// Lifestyle picker to be used when 
        /// </summary>
        public Func<Type, ICompiledLifestyle> AutoRegistrationLifestylePicker { get; set; }

        /// <summary>
        /// Export as type and base implementations, false by default
        /// </summary>
        public bool ExportAsBase { get; set; }

        /// <summary>
        /// Use custom disposal scopes
        /// </summary>
        public IDisposalScopeProvider DisposalScopeProvider { get; set; }

        /// <summary>
        /// Filter interfaces to be exported
        /// </summary>
        public Func<Type, Type, bool> ExportByInterfaceFilter { get; set; }

        /// <summary>
        /// When true the container will pass the current injection context into SingletonPerScope and SingletonPerNamedScope lifestyle.
        /// By default this is false and a new context is will be created if needed.
        /// </summary>
        public bool SingletonPerScopeShareContext { get; set; }

        /// <summary>
        /// Support Func&lt;Type,object&gt; out of the box
        /// </summary>
        public bool SupportFuncType { get; set; }

        /// <summary>
        /// Provide action that will be called for tracing purposes
        /// </summary>
        public Action<string> Trace { get; set; }

        /// <summary>
        /// Should the container track disposable transients
        /// </summary>
        public bool TrackDisposableTransients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool InjectIDisposable { get; set; }

        /// <inheritdoc />
        public bool ReturnKeyedInEnumerable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IInjectionScopeConfiguration Clone()
        {
            return new InjectionScopeConfiguration
            {
                AutoRegisterUnknown = AutoRegisterUnknown,
                Behaviors = Behaviors,
                CacheArraySize = CacheArraySize,
                CatchDisposalExceptions = CatchDisposalExceptions,
                DisposalScopeProvider = DisposalScopeProvider,
                ExportAsBase = ExportAsBase,
                ExportByInterfaceFilter = ExportByInterfaceFilter,
                ExportStrategyArraySize = ExportStrategyArraySize,
                InjectIDisposable = InjectIDisposable,
                Implementation = Implementation.Clone()
            };
        }

        /// <summary>
        /// Default interface filter
        /// </summary>
        /// <returns></returns>
        public static bool DefaultInterfaceFilter(Type interfaceType, Type implementingType)
        {
            return interfaceType == typeof(IDisposable) ||
                   interfaceType.Name == "_Attribute";
        }

        #region Default Configuration

        /// <summary>
        /// Default implementation factor
        /// </summary>
        protected static readonly ImplementationFactory DefaultImplementation;

        /// <summary>
        /// static constructor that sets up a default implementation for the DI container
        /// </summary>
        static InjectionScopeConfiguration()
        {
            DefaultImplementation = new ImplementationFactory();

            DefaultImplementation.ExportSingleton<IAttributeDiscoveryService>(f => new AttributeDiscoveryService());

            DefaultImplementation.ExportInstance<IActivationStrategyCollectionContainer<ICompiledExportStrategy>>(
                f => new ActivationStrategyCollectionContainer<ICompiledExportStrategy>(f.InjectionScope.ScopeConfiguration.ExportStrategyArraySize, f.InjectionScope.ScopeConfiguration.ExportAsBase));

            DefaultImplementation.ExportInstance<IActivationStrategyCollectionContainer<ICompiledWrapperStrategy>>(
                f => new ActivationStrategyCollectionContainer<ICompiledWrapperStrategy>(f.InjectionScope.ScopeConfiguration.ExportStrategyArraySize, f.InjectionScope.ScopeConfiguration.ExportAsBase));

            DefaultImplementation.ExportInstance<IActivationStrategyCollectionContainer<ICompiledDecoratorStrategy>>(
                f => new ActivationStrategyCollectionContainer<ICompiledDecoratorStrategy>(f.InjectionScope.ScopeConfiguration.ExportStrategyArraySize, f.InjectionScope.ScopeConfiguration.ExportAsBase));

            DefaultImplementation.ExportInstance<IActivationStrategyCompiler>(
                f => new ActivationStrategyCompiler(f.InjectionScope.ScopeConfiguration,
                                                    f.Locate<IActivationExpressionBuilder>(),
                                                    f.Locate<IAttributeDiscoveryService>(),
                                                    f.Locate<IDefaultStrategyExpressionBuilder>(),
                                                    f.Locate<IInjectionContextCreator>(),
                                                    f.Locate<IExpressionConstants>(),
                                                    f.Locate<IInjectionStrategyDelegateCreator>()));

            DefaultImplementation.ExportInstance<ICanLocateTypeService>(f => new CanLocateTypeService());

            DefaultImplementation.ExportInstance<IExportRegistrationBlockValueProvider>(
                f => new ExportRegistrationBlock(f.InjectionScope, f.Locate<IActivationStrategyCreator>()));

            DefaultImplementation.ExportInstance<IActivationStrategyCreator>(
                f => new ActivationStrategyProvider(f.InjectionScope, f.Locate<IDefaultStrategyExpressionBuilder>()));

            DefaultImplementation.ExportInstance<IDefaultStrategyExpressionBuilder>(
                f => new DefaultStrategyExpressionBuilder(f.Locate<ITypeExpressionBuilder>()));

            DefaultImplementation.ExportInstance<ITypeExpressionBuilder>(
                f => new TypeExpressionBuilder(f.Locate<IInstantiationExpressionCreator>(),
                                               f.Locate<IDisposalScopeExpressionCreator>(),
                                               f.Locate<IMemberInjectionExpressionCreator>(),
                                               f.Locate<IMethodInvokeExpressionCreator>(),
                                               f.Locate<IEnrichmentExpressionCreator>()));

            DefaultImplementation.ExportSingleton<IInstantiationExpressionCreator>(f => new InstantiationExpressionCreator());
            DefaultImplementation.ExportInstance<IDisposalScopeExpressionCreator>(f => new DisposalScopeExpressionCreator());
            DefaultImplementation.ExportInstance<IEnrichmentExpressionCreator>(f => new EnrichmentExpressionCreator());
            DefaultImplementation.ExportInstance<IMethodInvokeExpressionCreator>(f => new MethodInvokeExpressionCreator());
            DefaultImplementation.ExportInstance<IMemberInjectionExpressionCreator>(f => new MemberInjectionExpressionCreator());

            DefaultImplementation.ExportInstance<IActivationExpressionBuilder>(
                f => new ActivationExpressionBuilder(f.Locate<IArrayExpressionCreator>(),
                                                     f.Locate<IEnumerableExpressionCreator>(),
                                                     f.Locate<IWrapperExpressionCreator>(),
                                                     f.Locate<IInjectionContextValueProvider>()));

            DefaultImplementation.ExportInstance<IArrayExpressionCreator>(
                f => new ArrayExpressionCreator(f.Locate<IWrapperExpressionCreator>()));

            DefaultImplementation.ExportInstance<IEnumerableExpressionCreator>(
                f => new EnumerableExpressionCreator());

            DefaultImplementation.ExportInstance<IWrapperExpressionCreator>(
                f => new WrapperExpressionCreator());

            DefaultImplementation.ExportInstance(f => ExpressionConstants.Default);

            DefaultImplementation.ExportInstance<IMissingExportStrategyProvider>(
                f => new ConcreteExportStrategyProvider());

            DefaultImplementation.ExportInstance<IDefaultWrapperCollectionProvider>(
                f => new DefaultWrapperCollectionProvider());

            DefaultImplementation.ExportInstance<IInjectionContextCreator>(f => new InjectionContextCreator());
            DefaultImplementation.ExportInstance<IInjectionContextValueProvider>(f => new InjectionContextValueProvider());

            DefaultImplementation.ExportInstance<IActivationStrategyAttributeProcessor>(f => new ActivationStrategyAttributeProcessor());

            DefaultImplementation.ExportInstance<IDynamicArrayLocator>(f => new DynamicArrayLocator());
            DefaultImplementation.ExportInstance<IDynamicIEnumerableLocator>(f => new DynamicIEnumerableLocator());

            DefaultImplementation.ExportInstance<IInjectionStrategyDelegateCreator>(f => new InjectionStrategyDelegateCreator());

            DefaultImplementation.ExportInstance<IStrongMetadataInstanceProvider>(f => new StrongMetadataInstanceProvider());
        }

        #endregion
    }
}
