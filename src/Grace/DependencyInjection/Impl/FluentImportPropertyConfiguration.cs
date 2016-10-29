using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl
{
    public class FluentImportPropertyConfiguration<T, TProp> : IFluentImportPropertyConfiguration<T, TProp>
    {
        private readonly IFluentExportStrategyConfiguration<T> _strategy;
        private readonly MemberInjectionInfo _memberInjectionInfo;

        public FluentImportPropertyConfiguration(IFluentExportStrategyConfiguration<T> strategy, MemberInjectionInfo memberInjectionInfo)
        {
            _strategy = strategy;
            _memberInjectionInfo = memberInjectionInfo;
        }

        #region Property methods


        public IFluentImportPropertyConfiguration<T, TProp> Consider(ExportStrategyFilter consider)
        {
            throw new NotImplementedException();
        }

        public IFluentImportPropertyConfiguration<T, TProp> DefaultValue(TProp defaultValue)
        {
            _memberInjectionInfo.DefaultValue = defaultValue;

            return this;
        }

        public IFluentImportPropertyConfiguration<T, TProp> IsRequired(bool isRequired = true)
        {
            _memberInjectionInfo.IsRequired = isRequired;

            return this;
        }

        public IFluentImportPropertyConfiguration<T, TProp> LocateWithKey(object locateKey)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFluentExportStrategyConfiguration
        public IFluentExportStrategyConfiguration<T> Apply(Action<T> applyAction)
        {
            return _strategy.Apply(applyAction);
        }

        public IFluentExportStrategyConfiguration<T> As(Type type)
        {
            return _strategy.As(type);
        }

        public IFluentExportStrategyConfiguration<T> As<TInterface>()
        {
            return _strategy.As<TInterface>();
        }

        public IFluentExportStrategyConfiguration<T> AsKeyed<TInterface>(object key)
        {
            return _strategy.AsKeyed<TInterface>(key);
        }

        public IFluentExportStrategyConfiguration<T> ByInterfaces(Func<Type, bool> filter = null)
        {
            return _strategy.ByInterfaces(filter);
        }

        public IFluentExportStrategyConfiguration<T> DisposalCleanupDelegate(Action<T> disposalCleanupDelegate)
        {
            return _strategy.DisposalCleanupDelegate(disposalCleanupDelegate);
        }

        public IFluentExportStrategyConfiguration<T> ImportMembers(Func<MemberInfo, bool> selector = null)
        {
            return _strategy.ImportMembers(selector);
        }

        public IFluentImportPropertyConfiguration<T, TProp1> ImportProperty<TProp1>(Expression<Func<T, TProp1>> property)
        {
            return _strategy.ImportProperty(property);
        }

        public ILifestylePicker<IFluentExportStrategyConfiguration<T>> Lifestyle => _strategy.Lifestyle;

        public IFluentExportStrategyConfiguration<T> UsingLifestyle(ICompiledLifestyle lifestyle)
        {
            return _strategy.UsingLifestyle(lifestyle);
        }

        public IWhenConditionConfiguration<IFluentExportStrategyConfiguration<T>> When => _strategy.When;

        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<TParam> paramValue = null)
        {
            return _strategy.WithCtorParam(paramValue);
        }

        public IFluentWithCtorConfiguration<T, TParam> WithCtorParam<TParam>(Func<IExportLocatorScope, StaticInjectionContext, IInjectionContext, TParam> paramValue)
        {
            return _strategy.WithCtorParam(paramValue);
        }

        public IFluentExportStrategyConfiguration<T> WithMetadata(object key, object value)
        {
            return _strategy.WithMetadata(key, value);
        }

        #endregion
    }
}
