using Grace.DependencyInjection.Exceptions;
using System;
using System.Reflection;
using Grace.Data;
using Grace.Utilities;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Interface for getting data from extra data 
    /// </summary>
    public interface IInjectionContextValueProvider
    {
        /// <summary>
        /// Get data from injection context
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        object GetValueFromInjectionContext(IExportLocatorScope scope,Type type, object key, IInjectionContext context,
            bool isRequired);

        /// <summary>
        /// Get data from injection context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locator"></param>
        /// <param name="staticContext"></param>
        /// <param name="key"></param>
        /// <param name="dataProvider"></param>
        /// <param name="defaultValue"></param>
        /// <param name="useDefault"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        T GetValueFromInjectionContext<T>(
            IExportLocatorScope locator,
            StaticInjectionContext staticContext,
            object key,
            IInjectionContext dataProvider,
            object defaultValue,
            bool useDefault,
            bool isRequired);
    }

    /// <summary>
    /// Implementation for fetching data from context value
    /// </summary>
    public class InjectionContextValueProvider : IInjectionContextValueProvider
    {
        /// <summary>
        /// Get data from injection context
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public virtual object GetValueFromInjectionContext(IExportLocatorScope locator, Type type, object key, IInjectionContext context, bool isRequired)
        {
            object value = null;

            if (context != null)
            {
                GetValueFromExtraDataProvider(type, key, context, out value);

                if (value == null && context.ExtraData != null)
                {
                    if (type.GetTypeInfo().IsAssignableFrom(context.ExtraData.GetType().GetTypeInfo()))
                    {
                        value = context.ExtraData;
                    }
                    else
                    {
                        var delegateInstance = context.ExtraData as Delegate;

                        if (delegateInstance != null && delegateInstance.GetMethodInfo().ReturnType == type)
                        {
                            value = delegateInstance;
                        }
                    }
                }
            }

            if (value == null)
            {
                var currentLocator = locator;

                while (currentLocator != null)
                {
                    if (GetValueFromExtraDataProvider(type, key, currentLocator, out value))
                    {
                        break;
                    }

                    currentLocator = currentLocator.Parent;
                }
            }
            
            if (value != null)
            {
                if (value is Delegate)
                {
                    value =
                        ReflectionService.InjectAndExecuteDelegate(locator, new StaticInjectionContext(type), context, value as Delegate);
                }

                if (!(type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo())))
                {
                    try
                    {
                        value = Convert.ChangeType(value, type);
                    }
                    catch (Exception exp)
                    {
                        // to do fix up exception
                        throw new LocateException(new StaticInjectionContext(type), exp);
                    }
                }
            }
            else if (isRequired)
            {
                throw new LocateException(new StaticInjectionContext(type));
            }

            return value;
        }

        /// <summary>
        /// Get data from injection context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locator"></param>
        /// <param name="staticContext"></param>
        /// <param name="key"></param>
        /// <param name="dataProvider"></param>
        /// <param name="defaultValue"></param>
        /// <param name="useDefault"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public virtual T GetValueFromInjectionContext<T>(IExportLocatorScope locator,
                                                 StaticInjectionContext staticContext, 
                                                 object key,
                                                 IInjectionContext dataProvider, 
                                                 object defaultValue, 
                                                 bool useDefault, 
                                                 bool isRequired)
        {
            object value = null;

            if (dataProvider != null)
            {
                GetValueFromExtraDataProvider<T>(key, dataProvider, out value);

                if (value == null)
                {
                    if (dataProvider.ExtraData is T)
                    {
                        value = dataProvider.ExtraData;
                    }
                    else
                    {
                        var delegateInstance = dataProvider.ExtraData as Delegate;

                        if (delegateInstance != null && delegateInstance.GetMethodInfo().ReturnType == typeof(T))
                        {
                            value = delegateInstance;
                        }
                    }
                }
            }

            if (value == null)
            {
                var currentLocator = locator;

                while (currentLocator != null)
                {
                    if (GetValueFromExtraDataProvider<T>(key, currentLocator, out value))
                    {
                        break;
                    }

                    currentLocator = currentLocator.Parent;
                }
            }

            if (value == null && useDefault)
            {
                value = defaultValue;
            }

            if (value != null)
            {
                if (value is Delegate)
                {
                    value =
                        ReflectionService.InjectAndExecuteDelegate(locator, staticContext, dataProvider, value as Delegate);
                }

                if(!(value is T))
                {
                    try
                    {
                        if (typeof(T).IsConstructedGenericType &&
                            typeof(T).GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var type = typeof(T).GetTypeInfo().GenericTypeArguments[0];

                            if (type.GetTypeInfo().IsEnum)
                            {
                                value = Enum.ToObject(type, value);
                            }
                            else
                            {
                                value = Convert.ChangeType(value, typeof(T).GetTypeInfo().GenericTypeArguments[0]);
                            }
                        }
                        else
                        {
                            value = Convert.ChangeType(value, typeof(T));
                        }
                    }
                    catch (Exception exp)
                    {
                        // to do fix up exception
                        throw new LocateException(staticContext, exp);
                    }
                }
            }
            else if (isRequired && !useDefault)
            {
                throw new LocateException(staticContext);
            }

            return (T)value;
        }

        /// <summary>
        /// Get value from extra data provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataProvider"></param>
        /// <param name="tValue"></param>
        /// <returns></returns>
        protected virtual bool GetValueFromExtraDataProvider<T>(object key, IExtraDataContainer dataProvider, out object tValue)
        {
            object value = null;

            if (key != null)
            {
                value = dataProvider.GetExtraData(key);
            }

            if (value != null)
            {
                tValue = value;
                return true;
            }

            foreach (var o in dataProvider.KeyValuePairs)
            {
                if (o.Key is string stringKey && 
                    stringKey.StartsWith(UniqueStringId.Prefix))
                {
                    continue;
                }

                if (o.Value is T)
                {
                    tValue = o.Value;

                    return true;
                }

                if (o.Value is Delegate delegateInstance && 
                    delegateInstance.GetMethodInfo().ReturnType == typeof(T))
                {
                    tValue = o.Value;

                    return true;
                }
            }

            tValue = null;

            return false;
        }

        protected virtual bool GetValueFromExtraDataProvider(Type type, object key, IExtraDataContainer dataProvider, out object tValue)
        {
            object value = null;

            if (key != null)
            {
                value = dataProvider.GetExtraData(key);
            }

            if (value != null)
            {
                tValue = value;
                return true;
            }

            foreach (var o in dataProvider.Values)
            {
                if (type.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo()))
                {
                    tValue = o;

                    return true;
                }

                var delegateInstance = o as Delegate;

                if (delegateInstance != null &&
                    delegateInstance.GetMethodInfo().ReturnType == type)
                {
                    tValue = o;

                    return true;
                }
            }

            tValue = null;

            return false;
        }
    }
}
