using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Impl
{
    public partial class ExportTypeSetConfiguration : IExportTypeSetCtorParamConfiguration
    {
        private class WithCtorParamInfo
        {
            public Type ParameterType { get; set; }

            public IExportValueProvider ValueProvider { get; set; }

            public Func<Type, ExportStrategyFilter> ConsiderFunc { get; set; }

            public Func<Type, string> ImportNameFunc { get; set; }

            public Func<Type, bool> IsRequiredFunc { get; set; }

            public Func<Type, object> LocateWithKeyFunc { get; set; }

            public ILocateKeyValueProvider LocateKeyValueProvider { get; set; }

            public Func<Type, string> NamedFunc { get; set; }

            public Func<Type, IExportValueProvider> ValueProviderFunc { get; set; }
        }

        /// <summary>
        /// Adds a constructor param to exported types
        /// </summary>
        /// <typeparam name="TParam">constructor param type</typeparam>
        /// <param name="paramFunc">func to create constructor param</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetCtorParamConfiguration WithCtorParam<TParam>(Func<TParam> paramFunc = null)
        {
            FuncValueProvider<TParam> funcValueProvider = null;

            if (paramFunc != null)
            {
                funcValueProvider = new FuncValueProvider<TParam>(paramFunc);
            }

            withCtorParams.Add(new WithCtorParamInfo
                               {
                                   ParameterType = typeof(TParam),
                                   ValueProvider = funcValueProvider
                               });

            return this;
        }

        /// <summary>
        /// Adds a constructor param to exported types
        /// </summary>
        /// <typeparam name="TParam">constructor param type</typeparam>
        /// <param name="paramFunc">func to create constructor param</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetCtorParamConfiguration WithCtorParam<TParam>(Func<IInjectionScope, IInjectionContext, TParam> paramFunc)
        {
            FuncValueProvider<TParam> funcValueProvider = null;

            if (paramFunc != null)
            {
                funcValueProvider = new FuncValueProvider<TParam>(paramFunc);
            }

            withCtorParams.Add(new WithCtorParamInfo
                                    {
                                        ParameterType = typeof(TParam),
                                        ValueProvider = funcValueProvider
                                    });

            return this;
        }

        /// <summary>
        /// Adds a constructor param to exported types
        /// </summary>
        /// <param name="paramType">constructor parameter type</param>
        /// <param name="paramFunc">func to create constructor param</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetCtorParamConfiguration WithCtorParam(Type paramType, Func<IInjectionScope, IInjectionContext, object> paramFunc)
        {
            FuncValueProvider<object> funcValueProvider = null;

            if (paramFunc != null)
            {
                funcValueProvider = new FuncValueProvider<object>(paramFunc);
            }

            withCtorParams.Add(new WithCtorParamInfo
                                    {
                                        ParameterType = paramType,
                                        ValueProvider = funcValueProvider
                                    });

            return this;
        }

        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filterFunc">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IExportTypeSetCtorParamConfiguration.Consider(Func<Type, ExportStrategyFilter> filterFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].ConsiderFunc = filterFunc;
            }

            return this;
        }

        /// <summary>
        /// Name to use when resolving parameter
        /// </summary>
        /// <param name="importNameFunc"></param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IExportTypeSetCtorParamConfiguration.ImportName(Func<Type, string> importNameFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].ImportNameFunc = importNameFunc;
            }

            return this;
        }

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequiredFunc">is the parameter required</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IExportTypeSetCtorParamConfiguration.IsRequired(Func<Type, bool> isRequiredFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                if (isRequiredFunc == null)
                {
                    withCtorParams[index].IsRequiredFunc = t => true;
                }
                else
                {
                    withCtorParams[index].IsRequiredFunc = isRequiredFunc;
                }
            }

            return this;
        }

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateWithKeyFunc">ocate key</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IExportTypeSetCtorParamConfiguration.LocateWithKey(Func<Type, object> locateWithKeyFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].LocateWithKeyFunc = locateWithKeyFunc;
            }

            return this;
        }

        /// <summary>
        /// Locate with a key provider func
        /// </summary>
        /// <param name="locateWithKeyFunc">locate func</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetCtorParamConfiguration LocateWithKeyProvider(Func<IInjectionScope, IInjectionContext, Type, object> locateWithKeyFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].LocateKeyValueProvider = new FuncLocateKeyProvider(locateWithKeyFunc);
            }

            return this;
        }

        /// <summary>
        /// Locate with a key provider
        /// </summary>
        /// <param name="locateKeyValueProvider">key provider</param>
        /// <returns>configuration object</returns>
        public IExportTypeSetCtorParamConfiguration LocateWithKeyProvider(ILocateKeyValueProvider locateKeyValueProvider)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].LocateKeyValueProvider = locateKeyValueProvider;
            }

            return this;
        }

        /// <summary>
        /// Name of the parameter to resolve
        /// </summary>
        /// <param name="namedFunc"></param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IExportTypeSetCtorParamConfiguration.Named(Func<Type, string> namedFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].NamedFunc = namedFunc;
            }

            return this;
        }

        /// <summary>
        /// Provides a value for a constructor parameter
        /// </summary>
        /// <param name="valueProviderFunc">value provider for parameter</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IExportTypeSetCtorParamConfiguration.UsingValueProvider(Func<Type, IExportValueProvider> valueProviderFunc)
        {
            int index = withCtorParams.Count - 1;

            if (index >= 0)
            {
                withCtorParams[index].ValueProviderFunc = valueProviderFunc;
            }

            return this;
        }
    }
}
