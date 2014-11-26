using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData
{
    public interface IDataSource
    {
        object Next(Type type, string key, IDataRequestContext context, object constraints);
    }

    public interface IDataSource<T> : IDataSource
    {

    }
}
