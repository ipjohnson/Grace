using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.TestData
{
    public interface ITestDataProvider
    {
        T Create<T>(string testDataName = null, IDataRequestContext context = null, object constraints = null);
    }
}
