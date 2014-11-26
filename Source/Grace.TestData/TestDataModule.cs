using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.TestData.DataSources;
using Grace.TestData.Impl;

namespace Grace.TestData
{
    public class TestDataModule : IConfigurationModule
    {
        public void Configure(IExportRegistrationBlock registrationBlock)
        {
            RegisterDataSources(registrationBlock);

            registrationBlock.Export<DataSourcePicker>().As<IDataSourcePicker>().WithPriority(-1);
            registrationBlock.Export<RandomDataGeneratorService>().As<IRandomDataGeneratorService>().WithPriority(-1).Lifestyle.Singleton();
            registrationBlock.Export<TestDataProvider>().As<ITestDataProvider>();
        }

        private void RegisterDataSources(IExportRegistrationBlock registrationBlock)
        {
            registrationBlock.Export(typeof(GenericDataSource<>)).As(typeof(IDataSource<>));
            registrationBlock.Export(typeof(NullableDataSource<>)).ByInterfaces();

            foreach (Type exportedType in GetType().GetTypeInfo().Assembly.ExportedTypes)
            {
                Type exportInterface = exportedType.GetTypeInfo()
                    .ImplementedInterfaces.FirstOrDefault(
                        t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IDataSource<>));

                if (exportInterface != null)
                {
                    DataSourceAttribute[] attrs =
                        exportedType.GetTypeInfo().GetCustomAttributes<DataSourceAttribute>().ToArray();

                    var exportDef = registrationBlock.Export(exportedType);

                    if (attrs.Length > 0)
                    {
                        foreach (DataSourceAttribute dataSourceKeyAttribute in attrs)
                        {
                            string key = dataSourceKeyAttribute.Key;

                            if (!string.IsNullOrEmpty(key))
                            {
                                exportDef.AsKeyed(exportInterface, dataSourceKeyAttribute.Key.ToLowerInvariant());
                            }
                            else
                            {
                                exportDef.As(exportInterface);
                            }
                        }
                    }
                    else
                    {
                        exportDef.As(exportInterface);
                    }
                }
            }
        }
    }
}
