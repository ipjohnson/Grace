using Grace.DependencyInjection.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Grace.Data;

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
                value = GetValueFromExtraDataProvider<T>(key, dataProvider);

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

                while (currentLocator != null && value == null)
                {
                    value = GetValueFromExtraDataProvider<T>(key, currentLocator);

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
                        value = Convert.ChangeType(value, typeof(T));
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
        /// <returns></returns>
        protected virtual object GetValueFromExtraDataProvider<T>(object key, IExtraDataContainer dataProvider)
        {
            object value = null;

            if (key != null)
            {
                value = dataProvider.GetExtraData(key);
            }

            return value ?? dataProvider.Values.FirstOrDefault(o =>
            {
                if (o is T)
                {
                    return true;
                }

                var delegateInstance = o as Delegate;

                if (delegateInstance != null)
                {
                    return delegateInstance.GetMethodInfo().ReturnType == typeof(T);
                }

                return false;
            });
        }
    }
}
