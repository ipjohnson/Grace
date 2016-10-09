using System;
using System.Linq;
using System.Reflection;
using Grace.Data;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public class InjectionContext : IInjectionContext
    {
        private ImmutableHashTree<string, object> _extraDataProperties;
        private ImmutableHashTree<object, object> _extraDataValues = ImmutableHashTree<object, object>.Empty;

        private InjectionContext()
        {

        }

        public InjectionContext(object extraData)
        {
            ExtraData = extraData;
            SharedData = new InjectionContextSharedData();

            _extraDataProperties = extraData != null ? 
                ReflectionService.GetPropertiesFromObject(extraData) : 
                ImmutableHashTree<string, object>.Empty;
        }

        public object GetExtraData(object key)
        {
            var stringKey = key as string;

            if (stringKey != null)
            {
                return _extraDataProperties.GetValueOrDefault(stringKey) ??
                       _extraDataValues.GetValueOrDefault(key);
            }

            return _extraDataValues.GetValueOrDefault(key);
        }

        public void SetExtraData(object key, object newValue, bool replaceIfExists = true)
        {
            _extraDataValues = _extraDataValues.Add(key, newValue, (o, n) => replaceIfExists ? n : o);
        }

        public object ExtraData { get; private set; }

        public IInjectionContextSharedData SharedData { get; private set; }

        public object GetValueByType(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return _extraDataProperties.Values.FirstOrDefault(v => typeInfo.IsAssignableFrom(v.GetType().GetTypeInfo()));
        }

        public IInjectionContext Clone()
        {
            return new InjectionContext
            {
                _extraDataProperties = _extraDataProperties,
                _extraDataValues = _extraDataValues,
                SharedData = SharedData,
                ExtraData = ExtraData
            };
        }
    }
}
