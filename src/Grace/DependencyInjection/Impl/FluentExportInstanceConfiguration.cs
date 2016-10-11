using System;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    public class FluentExportInstanceConfiguration<T> : IFluentExportInstanceConfiguration<T>
    {
        private readonly IConfigurableActivationStrategy _exportConfiguration;

        public FluentExportInstanceConfiguration(IConfigurableActivationStrategy exportConfiguration)
        {
            _exportConfiguration = exportConfiguration;
        }


        public IFluentExportInstanceConfiguration<T> As(Type type)
        {
            _exportConfiguration.AddExportAs(type);

            return this;
        }

        public IFluentExportInstanceConfiguration<T> AsKeyed(Type type, object key)
        {
            _exportConfiguration.AddExportAsKeyed(type, key);

            return this;
        }

        public IFluentExportInstanceConfiguration<T> As<TInterface>()
        {
            _exportConfiguration.AddExportAs(typeof(TInterface));

            return this;
        }

        public IFluentExportInstanceConfiguration<T> AsKeyed<TExportType, TKey>(TKey key)
        {
            _exportConfiguration.AddExportAsKeyed(typeof(TExportType), key);

            return this;
        }

        public IFluentExportInstanceConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            _exportConfiguration.Lifestyle = lifestyle;

            return this;
        }

        public IWhenConditionConfiguration<IFluentExportInstanceConfiguration<T>> When
        {
            get
            {
                return new WhenConditionConfiguration<IFluentExportInstanceConfiguration<T>>(
                    condition => _exportConfiguration.AddCondition(condition), this);
            }
        }

        public ILifestylePicker<IFluentExportInstanceConfiguration<T>> Lifestyle
        {
            get
            {
                return new LifestylePicker<IFluentExportInstanceConfiguration<T>>(this, lifestyle => _exportConfiguration.Lifestyle = lifestyle);
            }
        }
    }
}
