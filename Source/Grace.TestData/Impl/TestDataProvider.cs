using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.Impl
{
    public class TestDataProvider : ITestDataProvider
    {
        private readonly IReadOnlyCollection<IDataSourcePicker> _dataSourcePicker;

        public TestDataProvider(IReadOnlyCollection<IDataSourcePicker> dataSourcePicker)
        {
            _dataSourcePicker = dataSourcePicker;
        }

        public T Create<T>(string requestName = null, IDataRequestContext context = null, object constraints = null)
        {
            IDataSource dataSource = null;

            if (context == null)
            {
                context = new DataRequestContext();
            }

            foreach (IDataSourcePicker dataSourcePicker in _dataSourcePicker)
            {
                dataSource = dataSourcePicker.GetDataSource<T>(requestName, context, constraints);

                if (dataSource != null)
                {
                    break;
                }
            }

            if (dataSource == null)
            {
                throw new Exception("Could not find find data source for type: " + typeof(T).FullName);
            }

            object returnValue = dataSource.Next(typeof(T), requestName, context, constraints);

            if (returnValue != null)
            {
                return (T)returnValue;
            }

            throw new Exception("Null value returned from data source: " + dataSource.GetType().FullName);
        }
    }
}
