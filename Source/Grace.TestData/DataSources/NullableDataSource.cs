using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    public class NullableDataSource<T> : IDataSource<T?> where T : struct
    {
        private readonly IDataSourcePicker _dataPicker;

        public NullableDataSource(IDataSourcePicker dataPicker)
        {
            _dataPicker = dataPicker;
        }

        public object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return _dataPicker.GetDataSource<T>(key, context, constraints);
        }
    }
}
