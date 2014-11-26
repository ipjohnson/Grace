using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData
{
    public static class IDataSourcePickerExtensions
    {
        public static T GetValue<T>(this IDataSourcePicker dataSourcePicker,
                                    string key,
                                    IDataRequestContext context,
                                    object constraintObject)
        {
            IDataSource dataSource = dataSourcePicker.GetDataSource<T>(key, context, constraintObject);

            if (dataSource == null)
            {
                throw new Exception("Could not find data source for T: " + typeof(T).FullName);
            }

            return (T)dataSource.Next(typeof(T), key, context, constraintObject);
        }
    }
}
