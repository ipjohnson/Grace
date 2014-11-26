using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grace.DependencyInjection;

namespace Grace.TestData.Impl
{
    public class DataSourcePicker : IDataSourcePicker
    {
        private readonly IExportLocator _locator;

        public DataSourcePicker(IExportLocator locator)
        {
            _locator = locator;
        }

        public IDataSource GetDataSource<T>(string key, IDataRequestContext context, object constraints)
        {
            if (key != null)
            {
                IDataSource returnValue = _locator.Locate<IDataSource<T>>(withKey: key.ToLowerInvariant());

                if (returnValue != null)
                {
                    return returnValue;
                }
            }

            return _locator.Locate<IDataSource<T>>();
        }
    }
}
