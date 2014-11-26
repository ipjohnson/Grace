using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl;

namespace Grace.TestData.DataSources
{
    public class GenericDataSource<T> : BaseDataSource<T>
    {
        private readonly IExportLocator _exportLocator;
        private readonly IDataSourcePicker _dataSourcePicker;
        private readonly MethodInfo _dataPickerGetDataSourceMethodInfo;

        public GenericDataSource(IExportLocator exportLocator, IDataSourcePicker dataSourcePicker)
        {
            _exportLocator = exportLocator;
            _dataSourcePicker = dataSourcePicker;
            _dataPickerGetDataSourceMethodInfo = typeof(IDataSourcePicker).GetRuntimeMethod("GetDataSource",
                new[]
                {
                    typeof(string),
                    typeof(IDataRequestContext),
                    typeof(object)
                });
        }

        public override object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            object returnValue = _exportLocator.Locate(type, CreateInjectionContext(constraints), withKey: key);

            if (returnValue != null)
            {
                PopulateObject(returnValue, context);
            }

            return returnValue;
        }

        private void PopulateObject(object returnValue, IDataRequestContext context)
        {
            foreach (PropertyInfo runtimeProperty in returnValue.GetType().GetRuntimeProperties())
            {
                if (!runtimeProperty.CanWrite ||
                     runtimeProperty.SetMethod.IsStatic ||
                    !runtimeProperty.SetMethod.IsPublic ||
                     runtimeProperty.SetMethod.GetParameters().Count() != 1)
                {
                    continue;
                }

                MethodInfo closedMethod =
                    _dataPickerGetDataSourceMethodInfo.MakeGenericMethod(runtimeProperty.PropertyType);

                IDataSource dataSource = (IDataSource)closedMethod.Invoke(_dataSourcePicker, new object[]
                                                                                             {
                                                                                                 runtimeProperty.Name,
                                                                                                 context,
                                                                                                 null
                                                                                             });

                if (dataSource != null)
                {
                    runtimeProperty.SetValue(returnValue,
                        dataSource.Next(runtimeProperty.PropertyType, runtimeProperty.Name, context, null));
                }
            }
        }

        private IInjectionContext CreateInjectionContext(object constraints)
        {
            IInjectionContext injectionContext = _exportLocator.CreateContext();

            if (constraints != null)
            {
                foreach (PropertyInfo runtimeProperty in constraints.GetType().GetRuntimeProperties())
                {
                    if (!runtimeProperty.CanRead ||
                         runtimeProperty.GetMethod.IsStatic ||
                         runtimeProperty.GetMethod.IsPublic ||
                         runtimeProperty.GetMethod.GetParameters().Any())
                    {
                        continue;
                    }

                    PropertyInfo localPropertyInfo = runtimeProperty;

                    if (InjectionKernel.ImportTypeByName(runtimeProperty.PropertyType))
                    {
                        injectionContext.Export(runtimeProperty.Name, (s, c) => localPropertyInfo.GetValue(constraints));
                    }
                    else
                    {
                        injectionContext.Export(runtimeProperty.PropertyType, (s, c) => localPropertyInfo.GetValue(constraints));
                    }
                }
            }

            return injectionContext;
        }
    }
}
