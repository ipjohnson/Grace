using System;

namespace Grace.DependencyInjection.Impl
{
    public class FluentWithCtorConfiguration<T,TParam> : FluentExportStrategyConfiguration<T>, IFluentWithCtorConfiguration<T,TParam>
    {
        private readonly ConstructorParameterInfo _constructorParameterInfo;

        public FluentWithCtorConfiguration(IConfigurableActivationStrategy exportConfiguration, Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext,TParam> exportFunc) : base(exportConfiguration)
        {
            exportConfiguration.ConstructorParameter(_constructorParameterInfo = new ConstructorParameterInfo(exportFunc) { ParameterType = typeof(TParam)});
        }

        public IFluentWithCtorConfiguration<T, TParam> Named(string name)
        {
            _constructorParameterInfo.ParameterName = name;

            return this;
        }

        public IFluentWithCtorConfiguration<T, TParam> Consider(ExportStrategyFilter filter)
        {
            _constructorParameterInfo.ExportStrategyFilter = filter;

            return this;
        }

        public IFluentWithCtorConfiguration<T, TParam> DefaultValue(TParam defaultValue)
        {
            _constructorParameterInfo.DefaultValue = defaultValue;

            return this;
        }

        public IFluentWithCtorConfiguration<T, TParam> DefaultValue(Func<TParam> defaultValueFunc)
        {
            return DefaultValue((locator, staticContext, provider) => defaultValueFunc());
        }

        public IFluentWithCtorConfiguration<T, TParam> DefaultValue(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> defaultValueFunc)
        {
            _constructorParameterInfo.DefaultValue = defaultValueFunc;

            return this;
        }

        public IFluentWithCtorConfiguration<T, TParam> IsRequired(bool isRequired = true)
        {
            _constructorParameterInfo.IsRequired = isRequired;

            return this;
        }

        public IFluentWithCtorConfiguration<T, TParam> LocateWithKey(object locateKey)
        {
            _constructorParameterInfo.LocateWithKey = locateKey;

            return this;
        }
    }
}
