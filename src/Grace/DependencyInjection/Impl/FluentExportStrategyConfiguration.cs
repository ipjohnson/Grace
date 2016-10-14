using System;
using System.Reflection;
using Grace.DependencyInjection.Conditions;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    public class FluentExportStrategyConfiguration : IFluentExportStrategyConfiguration
    {
        private readonly IConfigurableActivationStrategy _exportConfiguration;

        public FluentExportStrategyConfiguration(IConfigurableActivationStrategy exportConfiguration)
        {
            _exportConfiguration = exportConfiguration;
        }
        
        public IFluentExportStrategyConfiguration As(Type type)
        {
            _exportConfiguration.AddExportAs(type);

            return this;
        }

        public IFluentExportStrategyConfiguration ImportMembers(Func<MemberInfo, bool> selector = null)
        {
            _exportConfiguration.MemberInjectionSelector(new PublicMemeberInjectionSelector(selector));

            return this;
        }

        public ILifestylePicker<IFluentExportStrategyConfiguration> Lifestyle => new LifestylePicker<IFluentExportStrategyConfiguration>(this, lifestlye => UsingLifestyle(lifestlye));

        public IFluentExportStrategyConfiguration UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            _exportConfiguration.Lifestyle = lifestyle;

            return this;
        }

        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration> When
        {
            get
            {
                return new WhenConditionConfiguration<IFluentExportStrategyConfiguration>(condition => _exportConfiguration.AddCondition(condition), this);
            }
        }

        public IFluentExportStrategyConfiguration WithMetadata(object key, object value)
        {
            _exportConfiguration.SetMetadata(key, value);

            return this;
        }
    }

    public class FluentExportStrategyConfiguration<T> : IFluentExportStrategyConfiguration<T>
    {
        private readonly IConfigurableActivationStrategy _exportConfiguration;

        public FluentExportStrategyConfiguration(IConfigurableActivationStrategy exportConfiguration)
        {
            _exportConfiguration = exportConfiguration;
        }
        
        public IFluentExportStrategyConfiguration<T> Apply(Action<T> applyAction)
        {
            var enrichmentDelegate = new Func<IExportLocatorScope, T, T>((scope, t) =>
              {
                  applyAction(t);

                  return t;
              });

            _exportConfiguration.EnrichmentDelegate(enrichmentDelegate);

            return this;
        }

        public IFluentExportStrategyConfiguration<T> As(Type type)
        {
            _exportConfiguration.AddExportAs(type);
            return this;
        }

        public IFluentExportStrategyConfiguration<T> As<TInterface>()
        {
            _exportConfiguration.AddExportAs(typeof(TInterface));

            return this;
        }

        public IFluentExportStrategyConfiguration<T> AsKeyed<TInterface>(object key)
        {
            _exportConfiguration.AddExportAsKeyed(typeof(TInterface), key);

            return this;
        }

        public IFluentExportStrategyConfiguration<T> ByInterfaces(Func<Type, bool> filter)
        {
            if (_exportConfiguration.ActivationType.GetTypeInfo().IsInterface)
            {
                if (filter == null || filter(_exportConfiguration.ActivationType))
                {
                    _exportConfiguration.AddExportAs(_exportConfiguration.ActivationType);
                }
            }
            else
            {
                foreach (Type interfaceTypes in _exportConfiguration.ActivationType.GetTypeInfo().ImplementedInterfaces)
                {
                    if (filter != null && !filter(interfaceTypes))
                    {
                        continue;
                    }

                    if (_exportConfiguration.ActivationType.GetTypeInfo().IsGenericTypeDefinition)
                    {
                        _exportConfiguration.AddExportAs(interfaceTypes.GetGenericTypeDefinition());
                    }
                    else
                    {
                        _exportConfiguration.AddExportAs(interfaceTypes);
                    }
                }
            }

            return this;
        }

        public IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(Action<T> disposalCleanupDelegate)
        {
            _exportConfiguration.DisposalDelegate = disposalCleanupDelegate;

            return this;
        }

        public IFluentExportStrategyConfiguration<T> ImportMembers(Func<MemberInfo, bool> selector = null)
        {
            _exportConfiguration.MemberInjectionSelector(new PublicMemeberInjectionSelector(selector ?? (m => true)));

            return this;
        }

        public ILifestylePicker<IFluentExportStrategyConfiguration<T>> Lifestyle
        {
            get
            {
                return new LifestylePicker<IFluentExportStrategyConfiguration<T>>(this, lifeStyle => UsingLifestyle(lifeStyle));
            }
        }

        public IFluentExportStrategyConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            _exportConfiguration.Lifestyle = lifestyle;

            return this;
        }

        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration<T>> When
        {
            get
            {
                return new WhenConditionConfiguration<IFluentExportStrategyConfiguration<T>>(condition => _exportConfiguration.AddCondition(condition), this);
            }
        }
        
        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
        {
            if (paramValue != null)
            {
                return WithCtorParam((locator, context, data) => paramValue());
            }

            return new FluentWithCtorConfiguration<T, TParam>(_exportConfiguration, null);
        }

        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> paramValue)
        {
            return new FluentWithCtorConfiguration<T, TParam>(_exportConfiguration, paramValue);
        }

        public IFluentExportStrategyConfiguration<T> WithMetadata(object key, object value)
        {
            _exportConfiguration.SetMetadata(key, value);

            return this;
        }
    }
}
