using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData
{
    public interface IDataSourcePicker
    {
        IDataSource GetDataSource<T>(string key, IDataRequestContext context, object constraints);
    }
}
