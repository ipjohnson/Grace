using System;
using System.Collections.Generic;
using System.Linq;
using Grace.Data.Immutable;
using System.Reflection;
using Grace.Data;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    public class ExportTypeSetConfiguration : IExportTypeSetConfiguration, IExportStrategyProvider
    {
        private readonly IActivationStrategyCreator _strategyCreator;
        private readonly IEnumerable<Type> _typesToExport;
        private readonly GenericFilterGroup<Type> _whereFilter;

        private ImmutableLinkedList<Type> _basedOnTypes = ImmutableLinkedList<Type>.Empty;
        private ImmutableLinkedList<Type> _byInterface = ImmutableLinkedList<Type>.Empty;
        private ImmutableLinkedList<Func<Type, bool>> _byInterfaces = ImmutableLinkedList<Func<Type, bool>>.Empty;
        private ImmutableLinkedList<Func<Type, IEnumerable<Type>>> _byTypes = ImmutableLinkedList<Func<Type, IEnumerable<Type>>>.Empty;
        private ImmutableLinkedList<Func<Type, IEnumerable<Tuple<Type, object>>>> _byKeyedType = ImmutableLinkedList<Func<Type, IEnumerable<Tuple<Type, object>>>>.Empty;
        private ImmutableLinkedList<Func<Type, IEnumerable<ICompiledCondition>>> _conditions = ImmutableLinkedList<Func<Type, IEnumerable<ICompiledCondition>>>.Empty;
        private ImmutableLinkedList<Func<Type, bool>> _excludeFuncs = ImmutableLinkedList<Func<Type, bool>>.Empty;
        private ImmutableLinkedList<IActivationStrategyInspector> _inspectors = ImmutableLinkedList<IActivationStrategyInspector>.Empty;
        private Func<Type, ICompiledLifestyle> _lifestyleFunc;

        public ExportTypeSetConfiguration(IActivationStrategyCreator strategyCreator, IEnumerable<Type> typesToExport)
        {
            _strategyCreator = strategyCreator;
            _typesToExport = typesToExport;
            _whereFilter = new GenericFilterGroup<Type>(ShouldSkipType, ExcludeTypesFilter);
        }

        public IExportTypeSetConfiguration BasedOn(Type baseType)
        {
            if (baseType == null) throw new ArgumentNullException(nameof(baseType));

            _basedOnTypes = _basedOnTypes.Add(baseType);

            return this;
        }

        public IExportTypeSetConfiguration BasedOn<T>()
        {
            _basedOnTypes = _basedOnTypes.Add(typeof(T));

            return this;
        }

        public IExportTypeSetConfiguration ByInterface(Type interfaceType)
        {
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));

            _byInterface = _byInterface.Add(interfaceType);

            return this;
        }

        public IExportTypeSetConfiguration ByInterface<T>()
        {
            _byInterface = _byInterface.Add(typeof(T));

            return this;
        }

        public IExportTypeSetConfiguration ByInterfaces(Func<Type, bool> whereClause = null)
        {
            if (whereClause == null)
            {
                whereClause = type => true;
            }

            _byInterfaces = _byInterfaces.Add(whereClause);

            return this;
        }

        public IExportTypeSetConfiguration ByType()
        {
            return ByTypes(t => new[] { t });
        }

        public IExportTypeSetConfiguration ByTypes(Func<Type, IEnumerable<Type>> typeDelegate)
        {
            if (typeDelegate == null) throw new ArgumentNullException(nameof(typeDelegate));

            _byTypes = _byTypes.Add(typeDelegate);

            return this;
        }

        public IExportTypeSetConfiguration ByKeyedTypes(Func<Type, IEnumerable<Tuple<Type, object>>> keyedDelegate)
        {
            _byKeyedType = _byKeyedType.Add(keyedDelegate);

            return this;
        }

        public IExportTypeSetConfiguration Exclude(Func<Type, bool> exclude)
        {
            if (exclude == null) throw new ArgumentNullException(nameof(exclude));

            _excludeFuncs = _excludeFuncs.Add(exclude);

            return this;
        }

        public ILifestylePicker<IExportTypeSetConfiguration> Lifestyle
        {
            get { return new LifestylePicker<IExportTypeSetConfiguration>(this, lifestyle => UsingLifestyle(lifestyle)); }
        }

        public IExportTypeSetConfiguration UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            return UsingLifestyle(type => lifestyle.Clone());
        }

        public IExportTypeSetConfiguration UsingLifestyle(Func<Type, ICompiledLifestyle> lifestyleFunc)
        {
            if (lifestyleFunc == null) throw new ArgumentNullException(nameof(lifestyleFunc));

            _lifestyleFunc = lifestyleFunc;

            return this;
        }

        public IExportTypeSetConfiguration Where(Func<Type, bool> typeFilter)
        {
            if (typeFilter == null) throw new ArgumentNullException(nameof(typeFilter));

            _whereFilter.Add(typeFilter);

            return this;
        }

        public IExportTypeSetConfiguration WithInspector(IActivationStrategyInspector inspector)
        {
            if (inspector == null) throw new ArgumentNullException(nameof(inspector));

            _inspectors = _inspectors.Add(inspector);

            return this;
        }

        public IExportTypeSetConfiguration AndCondition(Func<Type, IEnumerable<ICompiledCondition>> conditionFunc)
        {
            if (conditionFunc == null) throw new ArgumentNullException(nameof(conditionFunc));

            _conditions = _conditions.Add(conditionFunc);

            return this;
        }

        public IWhenConditionConfiguration<IExportTypeSetConfiguration> When
        {
            get
            {
                return new WhenConditionConfiguration<IExportTypeSetConfiguration>(
                    condition => _conditions = _conditions.Add(t => new[] { condition }), this);
            }
        }

        public IEnumerable<ICompiledExportStrategy> ProvideExportStrategies()
        {
            if (_basedOnTypes != ImmutableLinkedList<Type>.Empty)
            {
                _whereFilter.Add(BasedOnTypesFilter);
            }

            var types = _typesToExport.Where(_whereFilter).ToArray();

            return CreateExportStrategiesForTypes(types);
        }

        private bool BasedOnTypesFilter(Type type)
        {
            foreach (var basedOnType in _basedOnTypes)
            {
                if (ReflectionService.CheckTypeIsBasedOnAnotherType(type, basedOnType))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<ICompiledExportStrategy> CreateExportStrategiesForTypes(Type[] types)
        {
            foreach (var type in types)
            {
                ImmutableLinkedList<Type> exportTypes = GetExportedTypes(type);
                ImmutableLinkedList<Tuple<Type, object>> keyedExports = GetKeyedExportTypes(type);

                if (exportTypes != ImmutableLinkedList<Type>.Empty ||
                    keyedExports != ImmutableLinkedList<Tuple<Type, object>>.Empty)
                {
                    yield return CreateExportStrategyForType(type, exportTypes, keyedExports);
                }
            }
        }

        private ICompiledExportStrategy CreateExportStrategyForType(Type type, ImmutableLinkedList<Type> exportTypes, ImmutableLinkedList<Tuple<Type, object>> keyedExports)
        {
            var strategy = _strategyCreator.GetCompiledExportStrategy(type);

            foreach (var exportType in exportTypes)
            {
                strategy.AddExportAs(exportType);
            }

            foreach (var keyedExport in keyedExports)
            {
                strategy.AddExportAsKeyed(keyedExport.Item1, keyedExport.Item2);
            }

            strategy.Lifestyle = _lifestyleFunc?.Invoke(type);

            if (_inspectors != ImmutableLinkedList<IActivationStrategyInspector>.Empty)
            {
                _inspectors.Visit(i => i.Inspect(strategy), true);
            }

            return strategy;
        }

        private ImmutableLinkedList<Tuple<Type, object>> GetKeyedExportTypes(Type type)
        {
            var returnList = ImmutableLinkedList<Tuple<Type, object>>.Empty;

            foreach (var exportFunc in _byKeyedType)
            {
                var types = exportFunc(type);

                if (types != null)
                {
                    returnList = returnList.AddRange(types);
                }
            }

            return returnList;
        }

        private ImmutableLinkedList<Type> GetExportedTypes(Type type)
        {
            var returnList = GetExportedInterfaceList(type);

            foreach (var typeFunc in _byTypes)
            {
                var types = typeFunc(type);

                if (types != null)
                {
                    returnList = returnList.AddRange(types);
                }
            }

            return returnList;
        }

        private ImmutableLinkedList<Type> GetExportedInterfaceList(Type type)
        {
            ImmutableLinkedList<Type> returnList = ImmutableLinkedList<Type>.Empty;

            foreach (var implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
            {
                foreach (Type exportInterface in _byInterface)
                {
                    if (exportInterface.GetTypeInfo().IsGenericTypeDefinition)
                    {
                        if (implementedInterface.IsConstructedGenericType &&
                             implementedInterface.GetGenericTypeDefinition() == exportInterface)
                        {
                            returnList = returnList.Add(type.GetTypeInfo().IsGenericTypeDefinition ? exportInterface : implementedInterface);
                        }
                    }
                    else if (exportInterface.GetTypeInfo().IsAssignableFrom(implementedInterface.GetTypeInfo()))
                    {
                        returnList = returnList.Add(exportInterface);
                    }
                }

                foreach (var interfaceFunc in _byInterfaces)
                {
                    if (interfaceFunc(implementedInterface))
                    {
                        returnList = returnList.Add(implementedInterface);
                    }
                }
            }

            return returnList;
        }

        private bool ExcludeTypesFilter(Type arg)
        {
            if (_excludeFuncs == ImmutableLinkedList<Func<Type, bool>>.Empty)
            {
                return true;
            }

            return !_excludeFuncs.Any(m => m(arg));
        }

        private static bool ShouldSkipType(Type exportedType)
        {
            bool skipType = exportedType.GetTypeInfo().IsInterface ||
                     exportedType.GetTypeInfo().IsAbstract ||
                     typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(exportedType.GetTypeInfo()) ||
                     typeof(Exception).GetTypeInfo().IsAssignableFrom(exportedType.GetTypeInfo());

            return !skipType;
        }
    }
}
