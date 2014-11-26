using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData.DataSources
{
    [DataSource]
    [DataSource(Key = "UniqueId")]
    public class GuidDataSource : IDataSource<Guid>
    {
        public object Next(Type type, string key, IDataRequestContext context, object constraints)
        {
            return Guid.NewGuid();
        }
    }
}
